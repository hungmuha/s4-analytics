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
                WARN_USER_VENDOR_EMAIL_CD AS WarnRequestorEmailCd,
                WARN_USER_AGNCY_EMAIL_CD AS WarnUserEmailCd,
                WARN_USER_EMAIL_CD AS WarnUserAgncyEmailCd,
                WARN_REQUESTOR_EMAIL_CD AS WarnUserVendorEmailCd
                FROM new_user_req_new";

            var results = conn.Query<NewUserRequest>(cmdText);
            return results;
        }

        public NewUserRequest GetNewUserRequestById(string id)
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
                WARN_USER_VENDOR_EMAIL_CD AS WarnRequestorEmailCd,
                WARN_USER_AGNCY_EMAIL_CD AS WarnUserEmailCd,
                WARN_USER_EMAIL_CD AS WarnUserAgncyEmailCd,
                WARN_REQUESTOR_EMAIL_CD AS WarnUserVendorEmailCd
                FROM new_user_req_new
                WHERE USER_ID = :ID";

            var results = conn.QueryFirstOrDefault<NewUserRequest>(cmdText, new { ID = id });
            return results;
        }

        public NewUserRequest GetNewUserRequestByReqNbr(int reqNbr)
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
                WARN_USER_VENDOR_EMAIL_CD AS WarnRequestorEmailCd,
                WARN_USER_AGNCY_EMAIL_CD AS WarnUserEmailCd,
                WARN_USER_EMAIL_CD AS WarnUserAgncyEmailCd,
                WARN_REQUESTOR_EMAIL_CD AS WarnUserVendorEmailCd
                FROM new_user_req_new
                WHERE REQ_NBR = :REQNBR";

            var results = conn.QueryFirstOrDefault<NewUserRequest>(cmdText, new { REQNBR = reqNbr });
            return results;
        }

        public void UpdateRequestQueue(NewUserRequest newUserReq)
        {

        }
    }
}
