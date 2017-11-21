using Dapper;
using Lib.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using S4Analytics.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace S4Analytics.Models
{
    public class NewUserRequestRepository
    {
        private string _applicationName;
        private string _connStr;
        private OracleConnection _conn;
        private string _membershipSchema;
        private SmtpClient _smtp;
        private string _globalAdminEmail;
        private string _supportEmail;
        private UserManager<S4IdentityUser<S4UserProfile>> _userManager;
        private string _newUserDocumentsUrl;
        private string _webinarUrl;

        public NewUserRequestRepository(
            IOptions<ServerOptions> serverOptions,
            UserManager<S4IdentityUser<S4UserProfile>> userManager)
        {
            _applicationName = serverOptions.Value.MembershipApplicationName;
            _connStr = serverOptions.Value.IdentityConnStr;
            _conn = new OracleConnection(_connStr);

            _membershipSchema = serverOptions.Value.OracleSchemas.Membership;

            _userManager = userManager;

            _smtp = new SmtpClient
            {
                EnableSsl = serverOptions.Value.EmailOptions.EnableSsl,
                Host = serverOptions.Value.EmailOptions.SmtpServer,
                Port = serverOptions.Value.EmailOptions.SmtpPort,
            };

            _globalAdminEmail = serverOptions.Value.EmailOptions.GlobalAdminEmail;
            _supportEmail = serverOptions.Value.EmailOptions.SupportEmail;
            _newUserDocumentsUrl = serverOptions.Value.NewUserDocumentsUrl;
            _webinarUrl = serverOptions.Value.WebinarUrl;
        }

        /// <summary>
        /// Return all records from NEW_USER_REQ
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<NewUserRequest>> GetAll(string adminUserName)
        {
            var adminUser = await _userManager.FindByNameAsync(adminUserName);
            var adminAgency = adminUser.Profile.Agency;

            var selectTxt = GetRequestSelectQuery();
            var cmdTxt = $@"{selectTxt}
                            FROM new_user_req u
                            LEFT JOIN s4_agncy a
                            ON u.agncy_id = a.agncy_id
                            LEFT JOIN contractor c
                            ON c.contractor_id = u.contractor_id";

            if (!adminUser.IsGlobalAdmin())
            {
                var whereClauses = new List<string>();

                // if none of the roles below apply, no results will be returned
                whereClauses.Add("1 = 0");

                if (adminUser.IsHSMVAdmin())
                {
                    // HSMV Admins can view all New Agency requests, and all New Vendor and New Consultant
                    // requests if the requesting agency is not an FDOT agency
                    whereClauses.Add($@"u.new_agncy_nm IS NOT NULL
                    OR (
                      u.contract_start_dt IS NOT NULL
                      AND a.agncy_nm NOT LIKE '%FDOT%'
                    )");
                }

                if (adminUser.IsFDOTAdmin())
                {
                    // FDOT Admins can view all New Vendor and New Consultant
                    // requests if the requesting agency is an FDOT agency
                    whereClauses.Add($@"u.contract_start_dt IS NOT NULL
                    AND a.agncy_nm LIKE '%FDOT%'");
                }

                if (adminUser.IsUserManager())
                {
                    // Agency User Managers can view New User requests from their agency, or
                    // if a parent agency, requests from its child agencies
                    if (adminAgency.ParentAgencyId == 0)
                    {
                        whereClauses.Add($@"u.contract_start_dt IS NULL
                        AND u.agncy_id = {adminAgency.AgencyId}");
                    }
                    else
                    {
                        whereClauses.Add($@"u.contract_start_dt IS NULL
                        AND u.agncy_id IN (
                          {adminAgency.AgencyId},
                          {adminAgency.ParentAgencyId}
                        )");
                    }
                }

                cmdTxt += " WHERE (" + string.Join(") OR (", whereClauses.ToArray()) + ")";
            }

            var results = _conn.Query<NewUserRequest>(cmdTxt);
            return results;
        }

        /// <summary>
        /// Return record from NEW_USER_REQ where REQ_NBR = reqNbr
        /// </summary>
        /// <param name="reqNbr">request number of record to return</param>
        /// <returns></returns>
        public NewUserRequest Find(int reqNbr)
        {
            var selectTxt = GetRequestSelectQuery();

            var cmdTxt = $@"{selectTxt}
                            FROM new_user_req u
                            LEFT JOIN s4_agncy a
                            ON u.agncy_id = a.agncy_id
                            LEFT JOIN contractor c
                            ON c.contractor_id = u.contractor_id
                            WHERE req_nbr = :reqnbr";

            var results = _conn.QueryFirstOrDefault<NewUserRequest>(cmdTxt, new { REQNBR = reqNbr });
            return results;
        }

        /// <summary>
        /// Create new user in S4_USER, USER_CNTY, USER_ROLE
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public async Task<NewUserRequest> ApproveNewUser(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.Request;

            var preferredUserName = (request.RequestorFirstNm[0] + request.RequestorLastNm).ToLower();
            var userName = await GenerateUserName(preferredUserName);

            var user = new S4IdentityUser<S4UserProfile>(userName);

            request.CreatedBy = approval.AdminUserName;
            user.Profile = CreateS4UserProfile(request);

            var passwordText = GenerateRandomPassword(8, 0);
            await _userManager.CreateAsync(user, passwordText);

            await _userManager.SetEmailAsync(user, user.Profile.EmailAddress);

            // TODO: need to be more generic here -hard coded for testing
            var roles = request.UserManagerCd
                ? new List<string> { "User", "User Manager" }
                : new List<string> { "User" };
            await _userManager.AddToRolesAsync(user, roles);

            // Send password cred to new user.
            var subject = "Signal Four Analytics user account created";
            var body = $@"<div>Dear {request.RequestorFirstNm}, <br><br>
                        Your Signal Four Analytics individual account has been created. 
                        You can access the system at <a href=""http://s4.geoplan.ufl.edu/"">http://s4.geoplan.ufl.edu.</a>
                        <br><br>To login, click on the Login link at the upper right corner of the screen
                        and enter the information below: <br><br>
                        username = {userName} <br>
                        password = {passwordText} <br><br>
                        Upon login you will be prompted to change your password. You will also be
                        prompted to read and accept the Signal Four Analytics user agreement before
                        using the system.
                        <br><br>Below are some links to some resources that should help familiarize you
                        with the system. <br><br>
                        <a href =""{_newUserDocumentsUrl}/S4_Analytics_FAQ.PDF"">S4 Analytics FAQ</a><br>
                        <a href=""{_newUserDocumentsUrl}/S4_Analytics_Slides_&_Recordings_10-6-2015.pdf"">S4 Analytics Training Slides</a><br>
                        <a href=""{_webinarUrl}"">S4 Analytics Training Webinar Recording</a>
                        <br><br>Please let us know if you need further assistance.<br><br></div>";

            var closing = GetEmailNotificationClosing();

            SendEmail(user.Profile.EmailAddress, null, _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.UserId = userName;
            request.UserCreatedDt = DateTime.Now;

            UpdateApprovedNewUserRequest(request);
            return request;
        }

        /// <summary>
        /// Create new user in S4_USER, USER_CNTY, USER_ROLE
        /// </summary>
        /// <param name="id"></param>
        /// <param name="before70days"></param>
        /// <param name="newStatus"></param>
        /// <param name="selectedRequest"></param>
        /// <returns></returns>
        public async Task<NewUserRequest> ApproveConsultant(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.Request;
            var before70days = approval.Before70Days;

            if (request.UserId != null)
            {
                return await ApproveExistingConsultant(id, approval);
            }

            var preferredUserName = (request.ConsultantFirstNm[0] + request.ConsultantLastNm).ToLower();
            var userName = await GenerateUserName(preferredUserName);

            var user = new S4IdentityUser<S4UserProfile>(userName);

            request.CreatedBy = approval.AdminUserName;
            request.ContractEndDt = approval.ContractEndDt;
            user.Profile = CreateS4UserProfile(request);
            user.Profile.CrashReportAccess = before70days ? CrashReportAccess.Within60Days : CrashReportAccess.After60Days;

            var passwordText = GenerateRandomPassword(8, 0);
            await _userManager.CreateAsync(user, passwordText);
            await _userManager.SetEmailAsync(user, user.Profile.EmailAddress);

            // TODO: need to be more generic here -hard coded for testing
            var roles = request.UserManagerCd
                ? new List<string> { "User", "User Manager" }
                : new List<string> { "User" };
            await _userManager.AddToRolesAsync(user, roles);

            // Send the approval the emails here.  Send password cred to new user.
            var subject = $@"Your Signal Four Analytics individual account as employee of {request.AgncyNm} has been created";

            var body = $@"<div>Dear {request.RequestorFirstNm}, <br><br>
                        Your Signal Four Analytics individual account has been created. 
                        You can access the system at <a href=""http://s4.geoplan.ufl.edu/"">http://s4.geoplan.ufl.edu.</a>
                        <br><br>To login, click on the Login link at the upper right corner of the screen
                        and enter the information below: <br><br>
                        username = {userName} <br>
                        password = {passwordText} <br><br>
                        Upon login you will be prompted to change your password. You will also be
                        prompted to read and accept the Signal Four Analytics user agreement before
                        using the system.<br><br>
                        Note that this account will expire on {((DateTime)request.ContractEndDt).ToShortDateString()}.
                        <br><br>Below are some links to some resources that should help familiarize you
                        with the system. <br><br>
                        <a href =""{_newUserDocumentsUrl}/S4_Analytics_FAQ.PDF"">S4 Analytics FAQ</a><br>
                        <a href=""{_newUserDocumentsUrl}/S4_Analytics_Slides_&_Recordings_10-6-2015.pdf"">S4 Analytics Training Slides</a><br>
                        <a href=""{_webinarUrl}"">S4 Analytics Training Webinar Recording</a>
                        <br><br>Please let us know if you need further assistance.<br><br></div>";

            var closing = GetEmailNotificationClosing();

            SendEmail(user.Profile.EmailAddress, null, _supportEmail, subject, body.ToString(), closing);

            request.RequestStatus = newStatus;
            request.AccessBefore70Days = before70days;
            request.UserId = userName;
            request.UserCreatedDt = DateTime.Now;
            UpdateApprovedNewUserRequest(request);
            return request;
        }

        public async Task<NewUserRequest> ApproveExistingConsultant(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.Request;
            var before70days = approval.Before70Days;
            var userName = request.UserId;

            var user = await _userManager.FindByNameAsync(userName);

            user.Profile.ForcePasswordChange = true;
            user.Profile.ModifiedBy = approval.AdminUserName;
            user.Profile.AccountStartDate = request.ContractStartDt;
            user.Profile.AccountExpirationDate = request.ContractEndDt;
            user.Profile.EmailAddress = request.ConsultantEmail;
            user.Profile.CrashReportAccess = before70days ? CrashReportAccess.Within60Days : CrashReportAccess.After60Days;
            await _userManager.UpdateAsync(user);

            var passwordText = GenerateRandomPassword(8, 0);
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, passwordText);

            await _userManager.SetEmailAsync(user, user.Profile.EmailAddress);

            var roles = request.UserManagerCd
                ? new List<string> { "User", "User Manager" }
                : new List<string> { "User" };
            await _userManager.AddToRolesAsync(user, roles);

            // Send the approval the emails here.  Send password cred to new user.
            var subject = $@"Your Signal Four Analytics individual account as employee of {request.AgncyNm} has been renewed";

            var body = $@"<div>Dear {request.ConsultantFirstNm}, <br><br>
                        Your Signal Four Analytics individual account has been renewed. 
                        You can access the system at http://s4.geoplan.ufl.edu/. <br><br>
                        To login click on the Login link at the upper right corner of the screen
                        and enter the information below: <br><br>
                        username = {userName} <br>
                        password = {passwordText} <br><br>
                        Upon login you will be prompted to change your password. You will also be 
                        prompted to read and accept the Signal Four Analytics user agreement before 
                        using the system.<br><br>
                        Note that this account will expire on {((DateTime)request.ContractEndDt).ToShortDateString()}. <br><br>
                        Please let me know if you need further assistance.<br><br></div>";

            var closing = GetEmailNotificationClosing();

            SendEmail(user.Profile.EmailAddress, null, _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.AccessBefore70Days = before70days;
            request.UserCreatedDt = DateTime.Now;
            request.CreatedBy = approval.AdminUserName;
            UpdateApprovedNewUserRequest(request);
            return request;
        }

        public NewUserRequest ApproveAgency(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.Request;
            var before70days = approval.Before70Days;

            // Nothing to update in database. Send email to global admin to create agency
            var subject = "New agency needs to be created in Signal Four Analytics";
            var body = $@"<div>{request.AgncyNm} has been approved for creation. Please create new agency with the following specs: 
                        <br><br>
                        &nbsp;&nbsp;Request Number = {request.RequestNbr} <br>
                        &nbsp;&nbsp;Agency Name = {request.AgncyNm} <br>
                        &nbsp;&nbsp;Access Before 70 days = {before70days} <br>
                        &nbsp;&nbsp;Email Domain = {request.AgncyEmailDomain}<br><br>
                        After creation, please go to Manage Requests in Signal Four Analytics to approve request #{request.RequestNbr}.<br><br></div>";

            var closing = GetEmailNotificationClosing();

            SendEmail(_globalAdminEmail, null, _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.AccessBefore70Days = before70days;

            UpdateApprovedNewUserRequest(request);
            return request;
        }

        /// <summary>
        /// Create new vendor in CONTRACTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="approval"></param>
        /// <returns></returns>
        public Task<NewUserRequest> ApproveNewVendor(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.Request;
            request.CreatedBy = approval.AdminUserName;

            //Create new Vendor in CONTRACTOR
            var vendor = CreateNewVendor(request);

            var result = StoreVendor(vendor);
            approval.Request.VendorId = vendor.VendorId;

            return ApproveConsultant(id, approval);
        }

        public async Task<NewUserRequest> ApproveCreatedNewAgency(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.Request;

            // Verify that new agency has been created.
            var newAgencyId = FindAgencyIdByName(request.AgncyNm);
            if (newAgencyId == 0)
            {
                // Agency has not been created (could not find an agency with matching name)
                return request;
            }

            approval.Request.AgncyId = newAgencyId;

            /// User will be created automatically after agency created because there is no one in
            /// the agency since its new. Therefore no one with an account in that agency to approve them
            return await ApproveNewUser(id, approval);
        }

        public NewUserRequest Reject(int id, RequestRejection rejection)
        {
            var newStatus = rejection.NewStatus;
            var request = rejection.Request;
            var rejectionReason = rejection.RejectionReason;

            // Send the rejection the email here.
            var subject = "Your request for a Signal Four Analytics individual account has been rejected";

            var body = $@"<div>
                Dear {request.RequestorFirstNm} <br><br>
                Your request for a new Signal 4 individual account for {request.ConsultantFirstNm ?? request.RequestorFirstNm}
                {request.ConsultantLastNm ?? request.RequestorLastNm} has been rejected. <br><br>
                The reason given: <br>
                {rejectionReason} <br><br>
                Please let me know if you need further assistance.<br><br></div>";

            var closing = GetEmailNotificationClosing();

            SendEmail(request.RequestorEmail, null, _supportEmail, subject, body.ToString(), closing);

            request.RequestStatus = newStatus;
            request.AdminComment = rejectionReason;
            UpdateRejectedNewUserRequest(request);
            return request;
        }

        public int FindAgencyIdByName(string agencyNm)
        {
            var selectTxt = @"SELECT AGNCY_ID FROM S4_AGNCY WHERE AGNCY_NM = :agencyNm";

            var result = _conn.QueryFirstOrDefault<int>(selectTxt, new { agencyNm });
            return result;
        }


        #region private methods

        private bool UpdateApprovedNewUserRequest(NewUserRequest request)
        {
            var updateTxt = @"UPDATE NEW_USER_REQ
                            SET
                                REQ_STATUS = :requestStatus,
                                AGNCY_ID = :agencyId,
                                CONTRACTOR_ID = :vendorId,
                                USER_CREATED_DT = :userCreatedDt,
                                USER_ID = :userId,
                                CONTRACT_END_DT = :contractEndDt
                            WHERE REQ_NBR = :requestNbr";

            // Do not write 0 when it should be null
            var agencyId = request.AgncyId == 0 ? (int?)null : request.AgncyId;
            var vendorId = request.VendorId == 0 ? (int?)null : request.VendorId;

            var rowsUpdated = _conn.Execute(updateTxt, new
            {
                request.RequestStatus,
                agencyId,
                vendorId,
                request.UserCreatedDt,
                request.UserId,
                request.ContractEndDt,
                request.RequestNbr
            });

            return rowsUpdated == 1;
        }

        private bool UpdateRejectedNewUserRequest(NewUserRequest request)
        {
            var updateTxt = @"UPDATE NEW_USER_REQ
                            SET
                                REQ_STATUS = :requestStatus,
                                ADMIN_COMMENT = :adminComment
                            WHERE REQ_NBR = :requestNbr";

            var rowsUpdated = _conn.Execute(updateTxt, new
            {
                request.RequestStatus,
                request.AdminComment,
                request.RequestNbr
            });

            return rowsUpdated == 1;
        }

        private string GetRequestSelectQuery()
        {
            return $@"SELECT
                            u.req_nbr AS requestnbr,
                            u.req_status AS requeststatus,
                            u.initial_req_status AS initialrequeststatus,
                            u.req_desc AS requestdesc,
                            u.req_type AS requesttype,
                            u.req_dt AS requestdt,
                            u.user_created_dt AS usercreateddt,
                            u.requestor_email_addr_tx AS requestoremail,
                            u.requestor_last_nm AS requestorlastnm,
                            u.requestor_first_nm AS requestorfirstnm,
                            u.requestor_suffix AS requestorsuffixnm,
                            CASE WHEN u.agncy_id IS NULL THEN u.new_agncy_nm ELSE a.agncy_nm END agncynm,
                            CASE WHEN u.agncy_id IS NULL THEN u.new_agncy_email_domain_tx ELSE a.email_domain END agncyemaildomain,
                            u.agncy_id AS agncyid,
                            u.user_id AS userid,
                            u.consultant_first_nm AS consultantfirstnm,
                            u.consultant_last_nm AS consultantlastnm,
                            u.consultant_suffix AS consultantsuffixnm,
                            u.consultant_email_addr_tx AS consultantemail,
                            u.contractor_id AS vendorid,
                            CASE WHEN u.contractor_id is NULL THEN u.new_contractor_nm ELSE c.contractor_nm END vendorname,
                            CASE WHEN u.contractor_id is NULL THEN u.new_contractor_email_domain_tx ELSE c.email_domain END vendoremaildomain,
                            u.access_reason_tx AS accessreasontx,
                            u.contract_start_dt AS contractstartdt,
                            u.contract_end_dt AS contractenddt,
                            CASE WHEN u.warn_requestor_email_cd = 'Y' THEN 1 ELSE 0 END AS warnrequestoremailcd,
                            CASE WHEN u.warn_consultant_email_cd = 'Y' THEN 1 ELSE 0 END AS warnconsultantemailcd,
                            CASE WHEN u.warn_duplicate_email_cd = 'Y' THEN 1 ELSE 0 END as warnduplicateemailcd,
                            CASE WHEN u.user_manager_cd = 'Y' THEN 1 ELSE 0 END AS usermanagercd,
                            u.admin_comment AS admincomment,
                            u.contract_pdf_nm AS contractPdfNm,
                            CASE WHEN (
                                SELECT count(*) FROM s4_user s
                                JOIN user_role r ON r.user_nm = s.user_nm AND r.role_nm = 'User Manager'
                                JOIN s4_agncy g ON g.agncy_id = s.agncy_id 
                                WHERE g.agncy_id = u.agncy_id) >  0 THEN 1 ELSE 0 END AS hasadmin,
                            CASE WHEN a.can_view = 1 THEN 1 ELSE 0 END AS accessbefore70days";
        }

        private S4UserProfile CreateS4UserProfile(NewUserRequest request)
        {
            S4UserProfile profile;

            switch (request.RequestType)
            {
                case NewUserRequestType.FlPublicAgencyEmployee:
                    profile = CreateEmployeeProfile(request);
                    break;
                case NewUserRequestType.FlPublicAgencyMgr:
                    profile = CreateConsultantProfile(request);
                    break;
                default:
                    return null;
            }

            profile.CreatedBy = request.CreatedBy;
            profile.CreatedDate = DateTime.Now;

            return profile;
        }

        /// <summary>
        /// Generate a unique user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private async Task<string> GenerateUserName(string userName)
        {
            var done = false;
            var increment = 0;
            var generatedUserName = userName;
            do
            {
                var result = await _userManager.FindByNameAsync(generatedUserName);
                if (result != null)
                {
                    increment++;
                    generatedUserName = userName + increment;
                }
                else { done = true; }
            }
            while (!done);

            return generatedUserName;
        }

        private bool StoreVendor(Vendor vendor)
        {
            int rowsInserted = 0;

            var insertText = @"INSERT INTO CONTRACTOR
                            (CONTRACTOR_ID, CONTRACTOR_NM, CREATED_BY, CREATED_DT, IS_ACTIVE, EMAIL_DOMAIN)
                            VALUES
                            (:vendorId, :vendorName, :createdBy, :createdDate, :isActive, :emailDomain)";

            rowsInserted += _conn.Execute(insertText,
                new
                {
                    vendor.VendorId,
                    vendor.VendorName,
                    vendor.CreatedBy,
                    vendor.CreatedDate,
                    isActive = vendor.IsActive ? "Y" : "N",
                    vendor.EmailDomain
                });

            return rowsInserted == 1;

        }

        private S4UserProfile CreateEmployeeProfile(NewUserRequest request)
        {
            var profile = new S4UserProfile()
            {
                EmailAddress = request.RequestorEmail,
                FirstName = request.RequestorFirstNm,
                LastName = request.RequestorLastNm,
                SuffixName = request.RequestorSuffixNm,
                CrashReportAccess = (request.AccessBefore70Days) ? CrashReportAccess.Within60Days : CrashReportAccess.After60Days,
                Agency = GetAgency(request.AgncyId),
                ForcePasswordChange = true
            };

            profile.ViewableCounties = profile.Agency.DefaultViewableCounties;
            profile.CrashReportAccess = profile.Agency.CrashReportAccess;
            return profile;
        }

        private List<UserCounty> GetViewableCountiesForConsultant(Agency agency)
        {
            if (agency.ParentAgencyId == 0)
            {
                return agency.DefaultViewableCounties;
            }

            // child agency inherit geographic area scope of its parent agency
            var parentAgency = GetAgency(agency.ParentAgencyId);
            return parentAgency.DefaultViewableCounties;
        }

        private S4UserProfile CreateConsultantProfile(NewUserRequest request)
        {
            var profile = new S4UserProfile()
            {
                EmailAddress = request.ConsultantEmail,
                FirstName = request.ConsultantFirstNm,
                LastName = request.ConsultantLastNm,
                SuffixName = request.ConsultantSuffixNm,
                AccountStartDate = request.ContractStartDt,
                AccountExpirationDate = request.ContractEndDt,
                CrashReportAccess = (request.AccessBefore70Days)?CrashReportAccess.Within60Days:CrashReportAccess.After60Days,
                Agency = GetAgency(request.AgncyId),
                VendorCompany = GetVendor(request.VendorId),
                ForcePasswordChange = true
            };

            profile.ViewableCounties = GetViewableCountiesForConsultant(profile.Agency);
            profile.TimeLimitedAccount = true;

            return profile;
        }

        private Vendor CreateNewVendor(NewUserRequest request)
        {
            var vendor = new Vendor(request.VendorName, GetNextVendorId())
            {
                CreatedBy = request.CreatedBy,
                CreatedDate = DateTime.Now,
                EmailDomain = request.VendorEmailDomain,
                IsActive = true
            };

            return vendor;
        }

        private Agency GetAgency(int agencyId)
        {
           var selectTxt = @"SELECT
                                agncy_id AS AGENCYID,
                                agncy_nm AS AGENCYNAME,
                                agncy_type_cd AS AGENCYTYPECD,
                                created_by AS CREATEDBY,
                                created_dt AS CREATEDDT,
                                CASE WHEN is_active = 'Y' THEN 1 ELSE 0 END AS ISACTIVE,
                                parent_agncy_id AS PARENTAGENCYID,
                                can_view AS CRASHREPORTACCESS,
                                email_domain AS EMAILDOMAIN
                                FROM S4_AGNCY WHERE AGNCY_ID = :agencyid";

            var agency = _conn.QueryFirstOrDefault<Agency>(selectTxt, new { agencyId });
            agency.DefaultViewableCounties = GetCountiesForAgency(agency.AgencyId);
            return agency;
        }

        private Vendor GetVendor(int id)
        {
            var selectTxt = @"SELECT
                                CONTRACTOR_ID as vendorId,
                                CONTRACTOR_NM as vendorName,
                                CREATED_BY as createdBy,
                                CREATED_DT as createdDate,
                                EMAIL_DOMAIN as emailDomain,
                                CASE WHEN IS_ACTIVE = 'Y' THEN 1 ELSE 0 END AS isActive
                              FROM CONTRACTOR
                              WHERE CONTRACTOR_ID = :id";

            var vendor = _conn.QueryFirstOrDefault<Vendor>(selectTxt, new { id });
            return vendor;
        }

        private List<UserCounty> GetCountiesForAgency(int agencyId)
        {
            var selectTxt = @"SELECT
                        cnty_cd AS CountyCode,
                        CASE WHEN can_edit = 'Y' THEN 1 ELSE 0 END AS CanEdit
                        FROM AGNCY_CNTY
                        WHERE AGNCY_ID = :agencyId";

            var results = _conn.Query<UserCounty>(selectTxt, new { agencyId });

            return results.ToList();
        }

        private int GetNextVendorId()
        {
            var selectTxt = @"SELECT SEQ_CONTRACTOR_ID.NEXTVAL FROM DUAL";

            var results = _conn.QueryFirstOrDefault<int>(selectTxt);
            return results;
        }

        private string GetEmailNotificationClosing()
        {
            var closing = $@"<div>Best regards,<br><br>
                            Signal Four Analytics<br>
                            University of Florida<br>
                            Email: {_supportEmail}";

            return closing;

        }

        private List<string> GetUserManagerEmails(int agencyId)
        {
            var emails = new List<string>();

            var selectTxt = string.Format( @"SELECT DISTINCT(U.EMAIL) FROM S4_USER U
                                JOIN USER_ROLE R ON R.ROLE_NM = 'User Manager'
                                AND R.USER_NM = U.USER_NM
                                JOIN {0}.ORA_ASPNET_USERS O ON O.USERNAME = U.USER_NM
                                JOIN {0}.ORA_ASPNET_MEMBERSHIP M ON M.USERID = O.USERID AND M.ISAPPROVED = 1
                                WHERE U.AGNCY_ID = :agencyId", _membershipSchema);

            var results = (_conn.Query<string>(selectTxt, new { agencyId })).ToList(); ;

            // if no admin for agency, send notification to s4 global admin
            if (results.Count == 0)
            {
                emails.Add(_globalAdminEmail);
            }

            return emails;
        }

        private List<string> GetFDOTAdminEmails()
        {
            var selectTxt = string.Format(@"SELECT DISTINCT(U.EMAIL) FROM S4_USER U
                                JOIN USER_ROLE R ON R.ROLE_NM = 'FDOT Admin'
                                AND R.USER_NM = U.USER_NM
                                JOIN {0}.ORA_ASPNET_USERS O ON O.USERNAME = U.USER_NM
                                JOIN {0}.ORA_ASPNET_MEMBERSHIP M ON M.USERID = O.USERID AND M.ISAPPROVED = 1", _membershipSchema);

            var emails = (_conn.Query<string>(selectTxt)).ToList();

            // if no FDOT admin, send notification to s4 global admin
            if (emails.Count == 0)
            {
                emails.Add(_globalAdminEmail);
            }

            return emails;
        }

        private List<string> GetHSMVAdminEmails()
        {
           var selectTxt = string.Format(@"SELECT DISTINCT(U.EMAIL) FROM S4_USER U
                                JOIN USER_ROLE R ON R.ROLE_NM = 'HSMV Admin'
                                AND R.USER_NM = U.USER_NM
                                JOIN {0}.ORA_ASPNET_USERS O ON O.USERNAME = U.USER_NM
                                JOIN {0}.ORA_ASPNET_MEMBERSHIP M ON M.USERID = O.USERID AND M.ISAPPROVED = 1", _membershipSchema);

            var emails = (_conn.Query<string>(selectTxt)).ToList();

            // if no FDOT admin, send notification to s4 global admin
            if (emails.Count == 0)
            {
                emails.Add(_globalAdminEmail);
            }

            return emails;
        }

        private bool IsFDOTRequest(NewUserRequest request)
        {
            return (request.AgncyNm.ToUpper()).Contains("FDOT");
        }

        private void SendEmail(string to, List<string> cc, string from, string subject, string body, string closing)
        {
            var fromEmail = new MailAddress(from);

            var msg = new MailMessage()
            {
                IsBodyHtml = true,
                Subject = subject,
                From = fromEmail
            };

            msg.To.Add(new MailAddress(to));

            if (cc != null)
            {
                foreach (string addr in cc)
                {
                    msg.CC.Add(new MailAddress(addr));
                }
            }

            var completedText = new StringBuilder(body);
            completedText.AppendLine("");
            completedText.AppendLine(closing);

            msg.Body = completedText.ToString();
            _smtp.Send(msg);
        }

        /// <summary>
        /// Utility function to generate a random password.
        /// This function exists purely for convenience. It does not implement any interface method.
        /// Modified and converted from Basic from http://www.4guysfromrolla.com/articles/101205-1.aspx
        /// </summary>
        /// <param name="length">Length of password to generate</param>
        /// <param name="numberOfNonAlphanumericCharacters">Number of non-alphanumeric characters to include in password</param>
        /// <returns>Generated password</returns>
        private string GenerateRandomPassword(int length, int numberOfNonAlphanumericCharacters)
        {
            int nonANcount = 0;
            byte[] buffer1 = new byte[length];

            //chPassword contains the password's characters as it's built up
            char[] chPassword = new char[length];

            //chPunctionations contains the list of legal non-alphanumeric characters
            char[] chPunctuations = "!@#$%^*()_-+=[{]};:|./?".ToCharArray();

            //Get a cryptographically strong series of bytes
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer1);

            for (int i = 0; i < length; i++)
            {
                //Convert each byte into its representative character
                int rndChr = buffer1[i] % 85;
                if (rndChr < 10)
                {
                    chPassword[i] = Convert.ToChar(Convert.ToUInt16(48 + rndChr));
                }
                else
                if (rndChr < 36)
                {

                    chPassword[i] = Convert.ToChar(Convert.ToUInt16((65 + rndChr) - 10));
                }
                else
                if (rndChr < 62)
                {

                    chPassword[i] = Convert.ToChar(Convert.ToUInt16((97 + rndChr) - 36));
                }
                else
                {
                    chPassword[i] = chPunctuations[rndChr - 62];
                    nonANcount += 1;
                }
            }

            if (nonANcount < numberOfNonAlphanumericCharacters)
            {
                Random rndNumber = new Random();
                for (int i = 0; i < (numberOfNonAlphanumericCharacters - nonANcount); i++)
                {
                    int passwordPos;
                    do
                    {
                        passwordPos = rndNumber.Next(0, length);
                    }
                    while (!char.IsLetterOrDigit(chPassword[passwordPos]));
                    chPassword[passwordPos] = chPunctuations[rndNumber.Next(0, chPunctuations.Length)];
                }
            }

            return new String(chPassword);
        }

        #endregion
    }
}
