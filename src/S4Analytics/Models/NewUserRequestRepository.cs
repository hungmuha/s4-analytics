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
using System.Threading;
using System.Threading.Tasks;

namespace S4Analytics.Models
{
    public class NewUserRequestRepository : INewUserRequestRepository
    {
        private string _applicationName;
        private string _connStr;
        private OracleConnection _conn;
        private IUserStore<S4IdentityUser<S4UserProfile>> _userStore;
        private IUserEmailStore<S4IdentityUser<S4UserProfile>> _userEmailStore;
        private IUserPasswordStore<S4IdentityUser<S4UserProfile>> _userPasswordStore;
        private IUserRoleStore<S4IdentityUser<S4UserProfile>> _userRoleStore;
        private IPasswordHasher<S4BaseUser> _passwordHasher;
        private SmtpClient _smtp;
        private string _globalAdminEmail;
        private string _supportEmail;

        public NewUserRequestRepository(
            IOptions<ServerOptions> serverOptions,
            IUserStore<S4IdentityUser<S4UserProfile>> userStore,
            IUserEmailStore<S4IdentityUser<S4UserProfile>> userEmailStore,
            IUserPasswordStore<S4IdentityUser<S4UserProfile>> userPasswordStore,
            IUserRoleStore<S4IdentityUser<S4UserProfile>> userRoleStore,
            IPasswordHasher<S4BaseUser> passwordHasher)
        {
            _applicationName = serverOptions.Value.MembershipApplicationName;
            _connStr = serverOptions.Value.WarehouseConnStr;
            _conn = new OracleConnection(_connStr);

            _userStore = userStore;
            _userEmailStore = userEmailStore;
            _userPasswordStore = userPasswordStore;
            _userRoleStore = userRoleStore;
            _passwordHasher = passwordHasher;

            _smtp = new SmtpClient
            {
                EnableSsl = serverOptions.Value.EmailOptions.EnableSsl,
                Host = serverOptions.Value.EmailOptions.SmtpServer,
                Port = serverOptions.Value.EmailOptions.SmtpPort,
            };

            _globalAdminEmail = serverOptions.Value.EmailOptions.GlobalAdminEmail;
            _supportEmail = serverOptions.Value.EmailOptions.SupportEmail;
        }

        /// <summary>
        /// Return all records from NEW_USER_REQ
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NewUserRequest> GetAll()
        {
            var selectTxt = GetRequestSelectQuery();
            var cmdTxt = $@"{selectTxt}
                            FROM new_user_req_new u
                            LEFT JOIN s4_agncy a
                            ON u.agncy_id = a.agncy_id
                            LEFT JOIN contractor c
                            ON c.contractor_id = u.contractor_id";

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
                            FROM new_user_req_new u
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
            var token = new CancellationToken();

            var newStatus = approval.NewStatus;
            var request = approval.SelectedRequest;

            var preferredUserName = (request.RequestorFirstNm[0] + request.RequestorLastNm).ToLower();
            var userName = GenerateUserName(preferredUserName);

            var user = new S4IdentityUser<S4UserProfile>(userName);

            user.Profile = CreateS4UserProfile(request);

            var passwordText = GenerateRandomPassword(8, 0);
            var passwordHash = _passwordHasher.HashPassword(user, passwordText);
            await _userPasswordStore.SetPasswordHashAsync(user, passwordHash, token);

            await _userEmailStore.SetEmailAsync(user, request.RequestorEmail, token);

            // TODO: need to be more generic here -hard coded for testing
            // TODO: If user is for a New Agency, then also need to create an Agency Admin role
            await _userRoleStore.AddToRoleAsync(user, "User", token);
            if (request.UserManagerCd)
            {
                await _userRoleStore.AddToRoleAsync(user, "Agency Admin", token);
            }

            var result = _userStore.CreateAsync(user, token);

            // Send password cred to new user.
            var subject = "Signal Four Analytics user account created";
            var body = string.Format(@"<div>Dear {0}, <br><br>
                        Your Signal Four Analytics individual account has been created. 
                        You can access the system at http://s4.geoplan.ufl.edu/. 
                        To login click on the Login link at the upper right of the screen 
                        and enter the information below: <br>
                        username = {1} <br>
                        password = {2} <br><br>
                        Upon login you will be prompted to change your password. You will also be 
                        prompted to read and accept Signal Four Analytics user agreement before 
                        using the system.<br><br>
                        Please let me know if you need further assistance.<br><br></div>", request.RequestorFirstNm, userName, passwordText);

            var closing = GetEmailNotificationClosing();

            SendEmail(user.Profile.EmailAddress, null, _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.UserId = userName;
            request.UserCreatedDt = DateTime.Now;
            request.CreatedBy = "tbd"; //TODO
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
        public async Task<NewUserRequest> ApproveNewConsultant(int id, RequestApproval approval)
        {
            var token = new CancellationToken();

            var newStatus = approval.NewStatus;
            var request = approval.SelectedRequest;
            var before70days = approval.Before70Days;

            // TODO: rename this method (ApproveNewConsultant is misleading because it also handles this case)
            if (request.UserId != null)
            {
                return await ApproveExistingConsultant(id, approval);
            }

            var preferredUserName = (request.RequestorFirstNm[0] + request.RequestorLastNm).ToLower();
            var userName = GenerateUserName(preferredUserName);

            var user = new S4IdentityUser<S4UserProfile>(userName);

            user.Profile = CreateS4UserProfile(request);
            user.Profile.CrashReportAccess = before70days ? CrashReportAccess.Within60Days : CrashReportAccess.After60Days;

            var passwordText = GenerateRandomPassword(8, 0);
            var passwordHash = _passwordHasher.HashPassword(user, passwordText);
            await _userPasswordStore.SetPasswordHashAsync(user, passwordHash, token);

            await _userEmailStore.SetEmailAsync(user, request.ConsultantEmail, token);

            // TODO: need to be more generic here -hard coded for testing
            // TODO: If user is for a New Agency, then also need to create an Agency Admin role
            await _userRoleStore.AddToRoleAsync(user, "User", token);
            if (request.UserManagerCd)
            {
                await _userRoleStore.AddToRoleAsync(user, "Agency Admin", token);
            }

            var result = _userStore.CreateAsync(user, token);

            // Send the approval the emails here.  Send password cred to new user.
            var subject = string.Format("Your Signal Four Analytics individual account as employee of " +
                 " {0} has been created", request.AgncyNm);

            var body = string.Format(@"<div>
                Dear {0} <br><br>
                Your Signal Four Analytics individual account has been created. 
                You can access the system at http://s4.geoplan.ufl.edu/. <br><br>
                To login click on the Login link at the upper right of the screen and enter the information below: <br><br>
                username: {1} <br>
                password: {2} <br><br>
                Upon login you will be prompted to change your password. You will also be
                prompted to read and accept Signal Four Analytics user agreement before
                using the system.<br><br>
                Note that this account will expire on {3}).
                Please let me know if you need further assistance.<br><br></div> ", request.ConsultantFirstNm, userName, passwordText, request.ContractEndDt.ToString());

            var closing = GetEmailNotificationClosing();

            SendEmail(user.Profile.EmailAddress, null, _supportEmail, subject, body.ToString(), closing);

            request.RequestStatus = newStatus;
            request.AccessBefore70Days = before70days;
            request.UserId = userName;
            request.UserCreatedDt = DateTime.Now;
            request.CreatedBy = "tbd"; //TODO
            UpdateApprovedNewUserRequest(request);
            return request;
        }

        public async Task<NewUserRequest> ApproveExistingConsultant(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.SelectedRequest;
            var before70days = approval.Before70Days;
            var userName = request.UserId;

            var token = new CancellationToken();
            var user = await _userStore.FindByNameAsync(userName, token);

            user.Profile.CrashReportAccess = before70days ? CrashReportAccess.Within60Days : CrashReportAccess.After60Days;

            var passwordText = GenerateRandomPassword(8, 0);
            var passwordHash = _passwordHasher.HashPassword(user, passwordText);
            await _userPasswordStore.SetPasswordHashAsync(user, passwordHash, token);

            await _userEmailStore.SetEmailAsync(user, request.ConsultantEmail, token);

            // TODO: need to be more generic here -hard coded for testing
            // TODO: If user is for a New Agency, then also need to create an Agency Admin role
            await _userRoleStore.AddToRoleAsync(user, "User", token);
            if (request.UserManagerCd)
            {
                await _userRoleStore.AddToRoleAsync(user, "Agency Admin", token);
            }

            var result = await _userStore.UpdateAsync(user, token);

            // Send the approval the emails here.  Send password cred to new user.
            var subject = string.Format("Your Signal Four Analytics individual account as employee of " +
                "{0} has been renewed", request.AgncyNm);

            var body = string.Format(@"<div>Dear {0}, <br><br>
                        Your Signal Four Analytics individual account has been renewed. 
                        You can access the system at http://s4.geoplan.ufl.edu/. <br><br>
                        To login click on the Login link at the upper right of the screen 
                        and enter the information below: <br><br>
                        username = {1} <br>
                        password = {2} <br><br>
                        Upon login you will be prompted to change your password. You will also be 
                        prompted to read and accept Signal Four Analytics user agreement before 
                        using the system.<br><br>
                        Note that this account will expire on {3}. <br><br>
                        Please let me know if you need further assistance.<br><br></div>", request.ConsultantFirstNm, userName, passwordText,
                        request.ContractEndDt.ToString());

            var closing = GetEmailNotificationClosing();

            SendEmail(user.Profile.EmailAddress, null, _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.AccessBefore70Days = before70days;
            request.UserCreatedDt = DateTime.Now;
            request.CreatedBy = "tbd"; //TODO
            UpdateApprovedNewUserRequest(request);
            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="before70days"></param>
        /// <param name="lea"></param>
        /// <param name="newStatus"></param>
        /// <param name="selectedRequest"></param>
        /// <returns></returns>
        public NewUserRequest ApproveAgency(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.SelectedRequest;
            var before70days = approval.Before70Days;
            var agencyType = request.AgncyTypeCd.ToString();
            var lea = approval.Lea;

            // Nothing to update in database. Send email to global admin to create agency
            var subject = "New agency needs to be created in Signal Four Analytics";
            var body = string.Format(@"<div>{0} has been approved for creation. Please create new agency. Below are the specs: <br><br>
                        Agency Name = {0} <br>
                        Access Before 70 days = {1} <br>
                        AgencyType = {2} <br>
                        Email Domain = {3}.<br><br></div>", request.AgncyNm, before70days, agencyType, request.AgncyEmailDomain);

            var closing = GetEmailNotificationClosing();

            SendEmail(_globalAdminEmail, null, _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.AccessBefore70Days = before70days;

            UpdateApprovedNewUserRequest(request);
            return request;
        }

        /// <summary>
        /// Create new contractor in CONTRACTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <param name="selectedRequest"></param>
        /// <returns></returns>
        public NewUserRequest ApproveNewContractor(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.SelectedRequest;

            //Create new Contractor in CONTRACTOR
            var contractor = CreateNewContractor(request);

            var result = StoreContractor(contractor);

            // Notify appropriate admin by email they need to approve user
            var adminEmails = IsFDOTRequest(request) ? GetFDOTAdminEmails() : GetHSMVAdminEmails();

            var subject = string.Format("New consultant working under {0} needs approval for Signal Four Account", request.AgncyNm);
            var body = string.Format(@"<div>There is a new request from {0} {1} from {2} for a contract with {3}.<br><br>
                    Please go to the User Request Queue in Signal Four Analytics
                    to review request and if ok, approve it.<br><br></div>", request.RequestorFirstNm, request.RequestorLastNm,
                    request.AgncyNm, request.ContractorNm);

            var closing = GetEmailNotificationClosing();

            SendEmail(adminEmails[0], adminEmails.GetRange(1, adminEmails.Count - 1), _supportEmail, subject, body, closing);

            request.RequestStatus = newStatus;
            request.ContractorId = contractor.ContractorId;
            UpdateApprovedNewUserRequest(request);
            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <param name="selectedRequest"></param>
        /// <returns></returns>
        public async Task<NewUserRequest> ApproveCreatedNewAgency(int id, RequestApproval approval)
        {
            var newStatus = approval.NewStatus;
            var request = approval.SelectedRequest;

            // Verify that new agency has been created.
            var newAgencyId = VerifyAgencyCreated(request.AgncyNm);
            if (newAgencyId == 0)
            {
                // Agency has not been created (could not find an agency with matching name)
                return null;
            }

            /// User will be created automatically after agency created because there is no one in
            /// the agency since its new. Therefore no one with an account in that agency to approve them
            return await ApproveNewUser(id, approval);
        }

        public NewUserRequest Reject(int id, RequestRejection rejection)
        {
            var newStatus = rejection.NewStatus;
            var request = rejection.SelectedRequest;
            var rejectionReason = rejection.RejectionReason;

            // Send the rejection the email here.
            var subject = string.Format("Your request for a Signal Four Analytics individual account has been rejected", request.AgncyNm);

            var body = string.Format(@"<div>
                Dear {0} <br><br>
                Your request for a new Signal 4 individual account for {1} {2} has been rejected. <br><br>
                The reason given: <br>
                {3} <br><br>
                Please let me know if you need further assistance.<br><br></div>",
                request.RequestorFirstNm, request.ConsultantFirstNm ?? request.RequestorFirstNm,
                request.ConsultantLastNm ?? request.RequestorLastNm,
                rejectionReason);

            var closing = GetEmailNotificationClosing();

            SendEmail(request.RequestorEmail, null, _supportEmail, subject, body.ToString(), closing);

            request.RequestStatus = newStatus;
            request.AdminComment = rejectionReason;
            UpdateRejectedNewUserRequest(request);
            return request;
        }

        #region private methods

        private bool UpdateApprovedNewUserRequest(NewUserRequest request)
        {
            var updateTxt = @"UPDATE NEW_USER_REQ_NEW
                            SET
                                REQ_STATUS = :requestStatus,
                                AGNCY_ID = :agncyId,
                                CONTRACTOR_ID = :contractorId,
                                USER_CREATED_DT = :userCreatedDt,
                                USER_ID = :userId
                            WHERE REQ_NBR = :requestNbr";

            var rowsUpdated = _conn.Execute(updateTxt, new
            {
                request.RequestStatus,
                request.AgncyId,
                request.ContractorId,
                request.UserCreatedDt,
                request.UserId,
                request.RequestNbr
            });

            return rowsUpdated == 1;
        }

        private bool UpdateRejectedNewUserRequest(NewUserRequest request)
        {
            var updateTxt = @"UPDATE NEW_USER_REQ_NEW
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
            return @"SELECT
                            u.req_nbr AS requestnbr,
                            u.req_status AS requeststatus,
                            u.req_desc AS requestdesc,
                            u.req_type AS requesttype,
                            u.req_dt AS requestdt,
                            u.user_created_dt AS usercreateddt,
                            u.requestor_email_addr_tx AS requestoremail,
                            u.requestor_last_nm AS requestorlastnm,
                            u.requestor_first_nm AS requestorfirstnm,
                            u.requestor_suffix AS requestorsuffixnm,
                            CASE WHEN u.agncy_id IS NULL OR u.agncy_id = 0 THEN u.new_agncy_nm ELSE a.agncy_nm END agncynm,
                            CASE WHEN u.agncy_id IS NULL OR u.agncy_id = 0 THEN u.new_agncy_type_cd ELSE a.agncy_type_cd END newagncytypecd,
                            CASE WHEN u.agncy_id IS NULL OR u.agncy_id = 0 THEN u.new_agncy_email_domain_tx ELSE a.email_domain END agncyemaildomain,
                            u.agncy_id AS agncyid,
                            u.user_id AS userid,
                            u.consultant_first_nm AS consultantfirstnm,
                            u.consultant_last_nm AS consultantlastnm,
                            u.consultant_suffix AS consultantsuffixnm,
                            u.consultant_email_addr_tx AS consultantemail,
                            u.contractor_id AS contractorid,
                            CASE WHEN u.contractor_id = 0 THEN u.new_contractor_nm ELSE c.contractor_nm END contractornm,
                            CASE WHEN u.contractor_id = 0 THEN u.new_contractor_email_domain_tx ELSE c.email_domain END ContractorEmailDomain,
                            u.access_reason_tx AS accessreasontx,
                            u.contract_end_dt AS contractstartdt,
                            u.contract_start_dt AS contractenddt,
                            CASE WHEN u.warn_requestor_email_cd = 'Y' THEN 1 ELSE 0 END AS warnrequestoremailcd,
                            CASE WHEN u.warn_consultant_email_cd = 'Y' THEN 1 ELSE 0 END AS warnconsultantemailcd,
                            CASE WHEN u.warn_duplicate_email_cd = 'Y' THEN 1 ELSE 0 END as warnduplicateemailcd,
                            CASE WHEN u.user_manager_cd = 'Y' THEN 1 ELSE 0 END AS usermanagercd,
                            u.admin_comment AS admincomment,
                            u.contract_pdf_nm AS contractPdfNm";
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

            profile.CreatedBy = "tbd"; // TODO: need to get currently logged in user name
            profile.CreatedDate = DateTime.Now;

            return profile;
        }

        /// <summary>
        /// Generate a unique user name
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private string GenerateUserName(string userName)
        {
            var done = false;
            var token = new CancellationToken();
            var increment = 0;
            var generatedUserName = userName;
            do
            {
                var result = _userStore.FindByNameAsync(generatedUserName, token);
                if (result.Result != null)
                {
                    increment++;
                    generatedUserName = userName + increment;
                }
                else { done = true; }
            }
            while (!done);

            return generatedUserName;

        }

        private bool StoreContractor(Contractor contractor)
        {
            int rowsInserted = 0;

            var insertText = @"INSERT INTO CONTRACTOR
                            (CONTRACTOR_ID, CONTRACTOR_NM, CREATED_BY, CREATED_DT, IS_ACTIVE, EMAIL_DOMAIN)
                            VALUES
                            (:contractorId, :contractorName, :createdBy, :createdDate, :isActive, :emailDomain)";

            rowsInserted += _conn.Execute(insertText,
                new
                {
                    contractor.ContractorId,
                    contractor.ContractorName,
                    contractor.CreatedBy,
                    contractor.CreatedDate,
                    isActive = contractor.IsActive ? "Y" : "N",
                    contractor.EmailDomain
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
                CrashReportAccess = (request.AccessBefore70Days)?CrashReportAccess.Within60Days:CrashReportAccess.After60Days,
                Agency = GetAgency(request.AgncyId),
                ForcePasswordChange = true
            };

            profile.ViewableCounties = profile.Agency.DefaultViewableCounties;
            profile.CrashReportAccess = profile.Agency.CrashReportAccess;
            return profile;
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
                ContractorCompany = GetContractor(request.ContractorId),
                ForcePasswordChange = true
            };

            profile.ViewableCounties = profile.Agency.DefaultViewableCounties;
            profile.TimeLimitedAccount = true;

            return profile;
        }

        private Contractor CreateNewContractor(NewUserRequest request)
        {
            var contractor = new Contractor(request.ContractorNm, GetNextContractorId())
            {
                CreatedBy = "tbd", //TODO
                CreatedDate = DateTime.Now,
                EmailDomain = request.ContractorEmailDomain,
                IsActive = true
            };

            return contractor;
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

        private Contractor GetContractor(int id)
        {
            var selectTxt = @"SELECT
                                CONTRACTOR_ID as contractorId,
                                CONTRACTOR_NM as contractorName,
                                CREATED_BY as createdBy,
                                CREATED_DT as createdDate,
                                EMAIL_DOMAIN as emailDomain,
                                CASE WHEN IS_ACTIVE = 'Y' THEN 1 ELSE 0 END AS isActive
                              FROM CONTRACTOR
                              WHERE CONTRACTOR_ID = :id";

            var contractor = _conn.QueryFirstOrDefault<Contractor>(selectTxt, new { id });
            return contractor;
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

        private int GetNextContractorId()
        {
            var selectTxt = @"SELECT SEQ_CONTRACTOR_ID.NEXTVAL FROM DUAL";

            var results = _conn.QueryFirstOrDefault<int>(selectTxt);
            return results;
        }

        private string GetEmailNotificationClosing()
        {
            var closing = string.Format(@"<div>Best regards,<br><br>
                            Signal Four Analytics<br>
                            University of Florida<br>
                            Email: {0}", _supportEmail);

            return closing;

        }

        private List<string> GetAgencyAdminEmails(int agencyId)
        {
            var emails = new List<string>();

            var selectTxt = @"SELECT DISTINCT(U.EMAIL) FROM S4_USER U
                                JOIN USER_ROLE R ON R.ROLE_NM = 'Agency Admin'
                                AND R.USER_NM = U.USER_NM
                                WHERE U.AGNCY_ID = :agencyId";

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
            var selectTxt = @"SELECT DISTINCT(U.EMAIL) FROM S4_USER U
                                JOIN USER_ROLE R ON R.ROLE_NM = 'FDOT Admin'
                                AND R.USER_NM = U.USER_NM";

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
           var selectTxt = @"SELECT DISTINCT(U.EMAIL) FROM S4_USER U
                                JOIN USER_ROLE R ON R.ROLE_NM = 'HSMV Admin'
                                AND R.USER_NM = U.USER_NM";

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

        private int VerifyAgencyCreated(string agencyNm)
        {
            var selectTxt = @"SELECT AGNCY_ID FROM S4_AGNCY WHERE AGNCY_NM = :agencyNm";

            var result = _conn.QueryFirstOrDefault<int>(selectTxt, new { agencyNm });
            return result;
        }

        private void SendEmail(string to, List<string> cc, string from, string subject, string body, string closing)
        {
            // TODO:  change back to send to correct people
            var msg = new MailMessage()
            {
                IsBodyHtml = true
            };
            var fromEmail = new MailAddress(from);
            msg.From = fromEmail;
            //msg.To.Add(new MailAddress(to));

            if (cc != null)
            {

                //foreach (string addr in cc)
                //{
                //    msg.CC.Add(new MailAddress(addr));
                //}
            }

            msg.To.Add(new MailAddress("mfowler@ufl.edu"));

            msg.Subject = subject;
            msg.IsBodyHtml = true;

            body += "\nTO: " + to + "\nCC: " + ((cc == null)? "": cc.ToString()) + "\n";

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
            char[] chPunctuations = "!@#$%^*()_-+=[{]};:<>|./?".ToCharArray();

            //Get a cryptographically strong series of bytes
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer1);

            for (int i = 0; i < length; i++)
            {
                //Convert each byte into its representative character
                int rndChr = buffer1[i] % 87;
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
