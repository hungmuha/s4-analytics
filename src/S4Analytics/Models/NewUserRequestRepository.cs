using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

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
                            u.new_contractor_nm AS newcontractornm,
                            u.new_contractor_email_domain_tx AS newcontractoremaildomain,
                            u.access_reason_tx AS accessreasontx,
                            u.contract_end_dt AS contractstartdt,
                            u.contract_start_dt AS contractenddt,
                            CASE WHEN u.warn_requestor_email_cd = 'Y' THEN 1 ELSE 0 END AS warnrequestoremailcd,
                            CASE WHEN u.warn_user_email_cd = 'Y' THEN 1 ELSE 0 END AS warnconsultantemailcd,
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
                            CASE WHEN u.warn_user_email_cd = 'Y' THEN 1 ELSE 0 END AS warnconsultantemailcd,
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

        public NewUserRequest  ApproveNewUser(int id, NewUserRequestStatus newStatus)
        {

            return null;
        }
        public NewUserRequest  ApproveNewConsultant(int id, bool before70days, NewUserRequestStatus newStatus)
        {
            return null;
        }

        public NewUserRequest  ApproveNewAgency(int id, bool before70days, bool lea, NewUserRequestStatus newStatus)
        {
            return null;
        }

        public NewUserRequest  ApproveNewContractor(int id, NewUserRequestStatus newStatus)
        {
            return null;
        }

        public NewUserRequest ApproveCreateNewAgency(int id, NewUserRequestStatus newStatus)
        {
            return null;
        }

    }
}
