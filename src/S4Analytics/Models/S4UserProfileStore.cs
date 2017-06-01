using Dapper;
using Lib.Identity;
using Lib.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace S4Analytics.Models
{
    public class S4UserProfileStore : IProfileStore<S4UserProfile>
    {
        private OracleConnection _conn;
        private string _applicationName;

        public S4UserProfileStore(OracleConnection conn, string applicationName)
        {
            _conn = conn;
            _applicationName = applicationName;
        }

        public Task<S4UserProfile> FindProfileForUserAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            // TODO: SELECT FROM S4_USER
            // TODO: SELECT FROM USER_CNTY

            throw new NotImplementedException();
        }

        public async Task<IdentityResult> CreateProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            // INSERT INTO S4_USER
            var insertTxt = @"INSERT INTO S4_USER
                (APPLICATION_NM, USER_NM, FIRST_NM, LAST_NM, NAME_SUFFIX,
                CREATED_BY, FORCE_PASSWORD_CHANGE,
                TIME_LIMITED_ACCOUNT_CD, ACCOUNT_START_DT, ACCOUNT_EXPIRATION_DT,
                AGNCY_ID, EMAIL, CREATED_DT, CAN_VIEW, CONTRACTOR_ID)
                VALUES (:appNm, :userName, :firstName, :lastName, :suffixName,
                :createdBy, :forcePasswordChange, :timeLimitedAccount,
                :accountStartDate, :accountExpirationDate, :agencyId, :emailAddress,
                :createdDate, :crashReportAccess, :contractorId)";

            await _conn.ExecuteAsync(insertTxt,
                new
                {
                    // TODO: add missing profile fields
                    user.UserName,
                    user.Profile.FirstName,
                    user.Profile.LastName,
                    user.Profile.SuffixName,
                    user.Profile.CreatedBy,
                    forcePasswordChange = user.Profile.ForcePasswordChange ? "Y" : "N",
                    timeLimitedAccount = user.Profile.TimeLimitedAccount ? "Y" : "N",
                    user.Profile.AccountStartDate,
                    user.Profile.AccountExpirationDate,
                    agencyId = user.Profile.Agency.AgencyId,
                    contractorId = user.Profile.ContractorCompany == null ? null : (int?)user.Profile.ContractorCompany.ContractorId,
                    emailAddress = user.Profile.EmailAddress,
                    createdDate = user.Profile.CreatedDate,
                    user.Profile.CrashReportAccess,
                    appNm = _applicationName
                });

            // INSERT INTO USER_CNTY
            insertTxt = @"INSERT INTO USER_CNTY
			(APPLICATION_NM, USER_NM, CNTY_CD, CAN_EDIT, CREATED_BY, CREATED_DT, MODIFIED_BY, MODIFIED_DT)
			VALUES (:appNm, :userName, :cntyCd, :canEdit, :createdBy, :createdDate, :modifiedBy, :modifiedDate)";

            foreach (UserCounty cnty in user.Profile.ViewableCounties)
            {
                await _conn.ExecuteAsync(insertTxt, new
                {
                    user.UserName,
                    cnty.CntyCd,
                    canEdit = cnty.CanEdit ? "Y" : "N",
                    cnty.CreatedBy,
                    cnty.CreatedDate,
                    cnty.ModifiedBy,
                    cnty.ModifiedDate,
                    appNm = _applicationName
                });
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            // UPDATE S4_USER
            var updateTxt = @"UPDATE S4_USER
                            SET ACCOUNT_START_DT = :accountStartDate,
                                ACCOUNT_EXPIRATION_DT = :accountExpirationDate,
                                MODIFIED_BY = :modifiedBy,
                                MODIFIED_DT = :modifiedDt,
                                FORCE_PASSWORD_CHANGE = :pwdChange,
                                CAN_VIEW = :reportAccess
                            WHERE USER_NM = :userName";

            await _conn.ExecuteAsync(updateTxt,
                new
                {
                    user.UserName,
                    user.Profile.AccountStartDate,
                    user.Profile.AccountExpirationDate,
                    modifiedDt = DateTime.Now,
                    modifiedBy = "tbd",
                    pwdChange = "Y",
                    reportAccess = user.Profile.CrashReportAccess
                }
             );

            // TODO: INSERT/UPDATE/DELETE USER_CNTY

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            // TODO: DELETE FROM USER_CNTY
            // TODO: DELETE FROM S4_USER

            throw new NotImplementedException();
        }
    }
}
