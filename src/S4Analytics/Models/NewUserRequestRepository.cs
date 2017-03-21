using Dapper;
using Lib.Identity;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace S4Analytics.Models
{
    public class NewUserRequestRepository : INewUserRequestRepository
    {
        private string _connStr;

        public NewUserRequestRepository(IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.WarehouseConnStr;
        }

        /// <summary>
        /// Return all records from NEW_USER_REQ
        /// </summary>
        /// <returns></returns>
        public IEnumerable<NewUserRequest> GetAll()
        {
            var conn = new OracleConnection(_connStr);

            var cmdText = @"SELECT 
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
                            a.agncy_nm AS agncynm,
                            u.new_agncy_nm AS newagncynm,
                            u.new_agncy_type_cd AS newagncytypecd,
                            u.agncy_id AS agncyid,
                            u.new_agncy_email_domain_tx AS newagncyemaildomain,
                            u.user_id AS userid,
                            u.consultant_first_nm AS consultantfirstnm,
                            u.consultant_last_nm AS consultantlastnm,
                            u.consultant_suffix AS consultantsuffixnm,
                            u.consultant_email_addr_tx AS consultantemail,
                            u.contractor_id AS contractorid,
                            c.contractor_nm as contractornm,
                            u.new_contractor_nm AS newcontractornm,
                            u.new_contractor_email_domain_tx AS newcontractoremaildomain,
                            u.access_reason_tx AS accessreasontx,
                            u.contract_end_dt AS contractstartdt,
                            u.contract_start_dt AS contractenddt,
                            CASE WHEN u.warn_requestor_email_cd = 'Y' THEN 1 ELSE 0 END AS warnrequestoremailcd,
                            CASE WHEN u.warn_consultant_email_cd = 'Y' THEN 1 ELSE 0 END AS warnconsultantemailcd,
                            u.admin_comment AS admincomment
                            FROM new_user_req_new u
                            LEFT JOIN s4_agncy a
                            ON u.agncy_id = a.agncy_id
                            LEFT JOIN contractor c
                            ON c.contractor_id = u.contractor_id";

            var results = conn.Query<NewUserRequest>(cmdText);
            return results;
        }

        /// <summary>
        /// Return record from NEW_USER_REQ where REQ_NBR = reqNbr
        /// </summary>
        /// <param name="reqNbr">request number of record to return</param>
        /// <returns></returns>
        public NewUserRequest Find(int reqNbr)
        {
            var conn = new OracleConnection(_connStr);

            var cmdText = @"SELECT 
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
                            a.agncy_nm AS agncynm,
                            u.new_agncy_nm AS newagncynm,
                            u.new_agncy_type_cd AS newagncytypecd,
                            u.agncy_id AS agncyid,
                            u.new_agncy_email_domain_tx AS newagncyemaildomain,
                            u.user_id AS userid,
                            u.consultant_first_nm AS consultantfirstnm,
                            u.consultant_last_nm AS consultantlastnm,
                            u.consultant_suffix AS consultantsuffixnm,
                            u.consultant_email_addr_tx AS consultantemail,
                            u.contractor_id AS contractorid,
                            u.new_contractor_nm AS newcontractornm,
                            u.new_contractor_email_domain_tx AS newcontractoremaildomain,
                            u.access_reason_tx AS accessreasontx,
                            u.contract_end_dt AS contractstartdt,
                            u.contract_start_dt AS contractenddt,
                            CASE WHEN u.warn_requestor_email_cd = 'Y' THEN 1 ELSE 0 END AS warnrequestoremailcd,
                            CASE WHEN u.warn_consultant_email_cd = 'Y' THEN 1 ELSE 0 END AS warnconsultantemailcd,
                            u.admin_comment AS admincomment
                            FROM new_user_req_new u
                            LEFT JOIN s4_agncy a
                            ON u.agncy_id = a.agncy_id
                            LEFT JOIN contractor c
                            ON c.contractor_id = u.contractor_id
                            WHERE req_nbr = :reqnbr";

            var results = conn.QueryFirstOrDefault<NewUserRequest>(cmdText, new { REQNBR = reqNbr });
            return results;
        }

        /// <summary>
        /// Create new user in S4_USER, USER_CNTY, USER_ROLE
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public NewUserRequest ApproveNewUser(int id, NewUserRequestStatus newStatus, NewUserRequest request)
        {
            var user = CreateIdentityUser(request);


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
        public NewUserRequest ApproveNewConsultant(int id, bool before70days, NewUserRequestStatus newStatus, NewUserRequest request)
        {
            return null;
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
        public NewUserRequest ApproveAgency(int id, bool before70days, bool lea, NewUserRequestStatus newStatus, NewUserRequest request)
        {
            return null;
        }

        /// <summary>
        /// Create new contractor in CONTRACTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <param name="selectedRequest"></param>
        /// <returns></returns>
        public NewUserRequest ApproveNewContractor(int id, NewUserRequestStatus newStatus, NewUserRequest request)
        {
            //Create new user in S4_USER, USER_CNTY, USER_ROLE
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <param name="selectedRequest"></param>
        /// <returns></returns>
        public NewUserRequest ApproveCreatedNewAgency(int id, NewUserRequestStatus newStatus, NewUserRequest request)
        {
            return null;
        }


        // CREATE CONSTRUCTOR IN LIB.IDENTITY TO TAKE A NEWUSEREQUEST AS A PARAMETER?
        private S4IdentityUser CreateIdentityUser(NewUserRequest request)
        {
            S4IdentityUser user;
            string email;
            string username;

            switch (request.RequestType)
            {
                case NewUserRequestType.FlPublicAgencyEmployee:
                    username = request.RequestorFirstNm[0] + request.RequestorLastNm;  // need to check name does not exist
                    email = request.RequestorEmail;
                    break;
                case NewUserRequestType.FlPublicAgencyMgr:
                    username = request.ConsultantFirstNm[0] + request.ConsultantLastNm;  // need to check name does not exist
                    email = request.ConsultantEmail;
                    break;
                default:
                    return null;
            }

            user = new S4IdentityUser(username, email);

            //pupulate remaining values
            // ....

            return user;
        }
    }
}
