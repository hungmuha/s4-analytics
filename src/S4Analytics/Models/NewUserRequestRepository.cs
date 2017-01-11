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
                REQ_NBR AS RequestNbr,
                REQ_STATUS AS RequestStatus,
                REQ_DESC AS RequestDesc,
                REQ_TYPE AS RequestType,
                REQ_DT AS RequestDt,
                USER_CREATED_DT AS UserCreatedDt,
                REQUESTOR_EMAIL_ADDR_TX AS RequestorEmail,
                REQUESTOR_LAST_NM AS RequestorLastNm,
                REQUESTOR_FIRST_NM AS RequestorFirstNm,
                REQUESTOR_SUFFIX AS RequestorSuffixNm,
                NEW_AGNCY_NM AS NewAgncyNm,
                NEW_AGNCY_TYPE_CD AS NewAgncyTypeCd,
                AGNCY_ID AS AgncyId,
                NEW_AGNCY_EMAIL_DOMAIN_TX AS NewAgncyEmailDomain,
                USER_ID AS UserId,
                CONSULTANT_FIRST_NM AS ConsultantFirstNm,
                CONSULTANT_LAST_NM AS ConsultantLastNm,
                CONSULTANT_SUFFIX AS ConsultantSuffixNm,
                CONSULTANT_EMAIL_ADDR_TX AS ConsultantEmail,
                CONTRACTOR_ID AS ContractorId,
                NEW_CONTRACTOR_NM AS NewContractorNm,
                NEW_CONTRACTOR_EMAIL_DOMAIN_TX AS NewContractorEmailDomain,
                ACCESS_REASON_TX AS AccessReasonTx,
                CONTRACT_END_DT AS ContractStartDt,
                CONTRACT_START_DT AS ContractEndDt,
                WARN_REQUESTOR_EMAIL_CD AS WarnRequestorEmailCd,                
                WARN_USER_EMAIL_CD AS WarnConsultantEmailCd,
                ADMIN_COMMENT as AdminComment
                FROM new_user_req_new";

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
                REQ_NBR AS RequestNbr,
                REQ_STATUS AS RequestStatus,
                REQ_DESC AS RequestDesc,
                REQ_TYPE AS RequestType,
                REQ_DT AS RequestDt,
                USER_CREATED_DT AS UserCreatedDt,
                REQUESTOR_EMAIL_ADDR_TX AS RequestorEmail,
                REQUESTOR_LAST_NM AS RequestorLastNm,
                REQUESTOR_FIRST_NM AS RequestorFirstNm,
                REQUESTOR_SUFFIX AS RequestorSuffixNm,
                NEW_AGNCY_NM AS NewAgncyNm,
                NEW_AGNCY_TYPE_CD AS NewAgncyTypeCd,
                AGNCY_ID AS AgncyId,
                NEW_AGNCY_EMAIL_DOMAIN_TX AS NewAgncyEmailDomain,
                USER_ID AS UserId,
                CONSULTANT_FIRST_NM AS ConsultantFirstNm,
                CONSULTANT_LAST_NM AS ConsultantLastNm,
                CONSULTANT_SUFFIX AS ConsultantSuffixNm,
                CONSULTANT_EMAIL_ADDR_TX AS ConsultantEmail,
                CONTRACTOR_ID AS ContractorId,
                NEW_CONTRACTOR_NM AS NewContractorNm,
                NEW_CONTRACTOR_EMAIL_DOMAIN_TX AS NewContractorEmailDomain,
                ACCESS_REASON_TX AS AccessReasonTx,
                CONTRACT_END_DT AS ContractStartDt,
                CONTRACT_START_DT AS ContractEndDt,
                WARN_REQUESTOR_EMAIL_CD AS WarnRequestorEmailCd,                
                WARN_USER_EMAIL_CD AS WarnConsultantEmailCd,
                ADMIN_COMMENT as AdminComment
                FROM new_user_req_new
                WHERE REQ_NBR = :REQNBR";

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

            foreach (KeyValuePair<string,object> kvp in body)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                switch (key)
                {
                    case "RequestStatus":
                        setStr += setStr == string.Empty ?
                            string.Format(" REQ_STATUS = {0} ", value):
                            string.Format(", REQ_STATUS = {0} ", value);
                        break;
                    case "UserCreatedDate":
                        setStr += setStr == string.Empty ?
                            string.Format(" USER_CREATED_DT = TO_DATE('{0}', 'MM-DD-YYYY') ", value) :
                            string.Format(", USER_CREATED_DT = TO_DATE('{0}', 'MM-DD-YYYY') ", value);
                        break;
                    case "Comment":
                        setStr += setStr == string.Empty ?
                            string.Format(" ADMIN_COMMENT = '{0}' ", value) :
                            string.Format(", ADMIN_COMMENT = '{0}' ", value);
                        break;
                    case "UserId":
                        setStr += setStr == string.Empty ?
                            string.Format(" USER_ID = '{0}' ", value) :
                            string.Format(", USER_ID = '{0}' ", value);
                        break;
                }
            }

            var cmdTxt = string.Format(@"UPDATE new_user_req_new SET {0}
                WHERE REQ_NBR = :REQNBR", setStr);

            var result = conn.Execute(cmdTxt, new { REQNBR = reqNbr });

            return result;
        }
    }
}
