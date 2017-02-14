using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
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
                            u.warn_requestor_email_cd AS warnrequestoremailcd,
                            u.warn_user_email_cd AS warnconsultantemailcd,
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
        /// Return all records from NEW_USER_REQ where REQ_STATUS = status
        /// </summary>
        /// <param name="reqNbr">request number of record to return</param>
        /// <returns></returns>
        public IEnumerable<NewUserRequest> FilterBy(NewUserRequestStatus status)
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
                            u.warn_requestor_email_cd AS warnrequestoremailcd,
                            u.warn_user_email_cd AS warnconsultantemailcd,
                            u.admin_comment AS admincomment
                            FROM new_user_req_new u
                            LEFT JOIN s4_agncy a
                            ON u.agncy_id = a.agncy_id
                            LEFT JOIN contractor c
                            ON c.contractor_id = u.contractor_id
                            WHERE req_status = :status";

            var results = conn.Query<NewUserRequest>(cmdText, new { STATUS = 10 });
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
                            u.warn_requestor_email_cd AS warnrequestoremailcd,
                            u.warn_user_email_cd AS warnconsultantemailcd,
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
        /// Update record in NEW_USER_REQ table
        /// </summary>
        /// <param name="reqNbr">record to update</param>
        /// <param name="body">fields to update</param>
        /// <returns>number of records updated</returns>
        public int Update(int reqNbr, Dictionary<string, object> body)
        {
            var conn = new OracleConnection(_connStr);

            var setStr = string.Empty;
            var p = new Dictionary<string, object> { { "REQNBR", reqNbr } };

            foreach (KeyValuePair<string, object> kvp in body)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                switch (key)
                {
                    case "RequestStatus":
                        setStr += setStr == string.Empty ?
                            string.Format(" req_status = :reqStatus") :
                            string.Format(", req_status = :reqStatus");
                        p.Add("REQSTATUS", value);
                        break;
                    case "UserCreatedDate":
                        setStr += setStr == string.Empty ?
                            string.Format(" user_created_dt = :createDt") :
                            string.Format(", user_created_dt = :createDt");
                        p.Add("CREATEDT", Convert.ToDateTime(value));
                        break;
                    case "Comment":
                        setStr += setStr == string.Empty ?
                            string.Format(" admin_comment = :adminComment"):
                            string.Format(", admin_comment = :adminComment");
                        p.Add("ADMINCOMMENT", value);
                        break;
                    case "UserId":
                        setStr += setStr == string.Empty ?
                            string.Format(" user_id = :userId") :
                            string.Format(", user_id = :userId");
                        p.Add("USERID", value);
                        break;
                }
            }

            var cmdTxt = string.Format(@"UPDATE new_user_req_new SET {0}
                WHERE req_nbr = :reqnbr", setStr);

            var result = conn.Execute(cmdTxt, p);// new { REQNBR = reqNbr });
            return result;
        }

    }
}
