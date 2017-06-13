using Dapper;
using Lib.Identity;
using Lib.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S4Analytics.Models
{
    public class S4UserProfileStore : IProfileStore<S4UserProfile>, IDisposable
    {
        private OracleConnection _connection;
        private string _applicationName;
        private bool _connectionCreatedInternally;

        public S4UserProfileStore(string applicationName, string connectionString)
        {
            _applicationName = applicationName;
            _connectionCreatedInternally = true;
            _connection = new OracleConnection(connectionString);
        }

        public S4UserProfileStore(string applicationName, OracleConnection connection)
        {
            _applicationName = applicationName;
            _connection = connection;
        }

        public void Dispose()
        {
            if (_connectionCreatedInternally)
            {
                _connection.Close();
            }
        }

        public async Task<S4UserProfile> FindProfileForUserAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var selectText = @"BEGIN
              OPEN :q1 FOR
                SELECT
                FIRST_NM AS FirstName,
                LAST_NM AS LastName,
                NAME_SUFFIX AS SuffixName,
                CREATED_BY AS CreatedBy,
                MODIFIED_BY AS ModifiedBy,
                MODIFIED_DT AS ModifiedDate,
                CASE FORCE_PASSWORD_CHANGE WHEN 'Y' THEN 1 ELSE 0 END AS ForcePasswordChange,
                CASE TIME_LIMITED_ACCOUNT_CD WHEN 'Y' THEN 1 ELSE 0 END AS TimeLimitedAccount,
                ACCOUNT_START_DT AS AccountStartDate,
                ACCOUNT_EXPIRATION_DT AS AccountExpirationDate,
                EMAIL AS EmailAddress,
                CREATED_DT AS CreatedDate,
                CAN_VIEW AS CrashReportAccess
                FROM S4_USER
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName;
              OPEN :q2 FOR
                SELECT
                  uc.CNTY_CD AS CountyCode,
                  s4_cnty.cnty_nm AS CountyName,
                  uc.CAN_VIEW AS CrashReportAccess,
                  CASE uc.CAN_EDIT WHEN 'Y' THEN 1 ELSE 0 END AS CanEdit,
                  uc.CREATED_BY AS CreatedBy,
                  uc.CREATED_DT AS CreatedDate,
                  uc.MODIFIED_BY AS ModifiedBy,
                  uc.MODIFIED_DT AS ModifiedDate
                FROM
                  USER_CNTY uc
                INNER JOIN S4_CNTY
                  ON S4_CNTY.CNTY_CD = uc.CNTY_CD
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName;
              OPEN :q3 FOR
                SELECT
                ag.agncy_id AS AGENCYID,
                ag.agncy_nm AS AGENCYNAME,
                ag.agncy_type_cd AS AGENCYTYPECD,
                ag.created_by AS CREATEDBY,
                ag.created_dt AS CREATEDDT,
                CASE WHEN ag.is_active = 'Y' THEN 1 ELSE 0 END AS ISACTIVE,
                ag.parent_agncy_id AS PARENTAGENCYID,
                ag.can_view AS CRASHREPORTACCESS,
                ag.email_domain AS EMAILDOMAIN
                FROM S4_AGNCY ag
                INNER JOIN s4_user u
                    ON u.agncy_id = ag.agncy_id
                WHERE APPLICATION_NM = :appName
                AND u.user_nm = :userName;
              OPEN :q4 FOR
                SELECT
                co.CONTRACTOR_ID as contractorId,
                co.CONTRACTOR_NM as contractorName,
                co.CREATED_BY as createdBy,
                co.CREATED_DT as createdDate,
                co.EMAIL_DOMAIN as emailDomain,
                CASE WHEN co.IS_ACTIVE = 'Y' THEN 1 ELSE 0 END AS isActive
                FROM CONTRACTOR co
                INNER JOIN s4_user u
                ON u.contractor_id = co.contractor_id
                WHERE APPLICATION_NM = :appName
                AND u.user_nm = :userName;
              OPEN :q5 FOR
                SELECT
                  AGREEMENT_NM AS AgreementName,
                  AGREEMENT_SIGNED_DT AS SignedDate,
                  AGREEMENT_EXPIRATION_DT AS ExpirationDate
                FROM
                  USER_AGREEMENT
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName;
            END;";

            string[] refCursorNames = { "q1", "q2", "q3", "q4", "q5" };
            var queryParams = new OracleDynamicParameters(new { appName = _applicationName, user.UserName }, refCursorNames);

            S4UserProfile profile;
            using (var multi = await _connection.QueryMultipleAsync(selectText, queryParams))
            {
                profile = multi.ReadFirstOrDefault<S4UserProfile>();
                if (profile != null)
                {
                    profile.ViewableCounties = multi.Read<UserCounty>().ToList();
                    profile.Agency = multi.ReadFirstOrDefault<Agency>();
                    profile.ContractorCompany = multi.ReadFirstOrDefault<Contractor>();
                    profile.Agreements = multi.Read<UserAgreement>().ToList();
                }
            }

            return profile;
        }

        public async Task<IdentityResult> CreateProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user.Profile == null)
            {
                // return IdentityResult.Failed(new IdentityError() { Description = "User profile cannot be null." });
                throw new Exception("User profile cannot be null."); // temporary
            }

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

            await _connection.ExecuteAsync(insertTxt,
                new
                {
                    user.UserName,
                    user.Profile.FirstName,
                    user.Profile.LastName,
                    user.Profile.SuffixName,
                    createdBy = "TBD",
                    createdDate = DateTime.Now,
                    forcePasswordChange = user.Profile.ForcePasswordChange ? "Y" : "N",
                    timeLimitedAccount = user.Profile.TimeLimitedAccount ? "Y" : "N",
                    user.Profile.AccountStartDate,
                    user.Profile.AccountExpirationDate,
                    agencyId = user.Profile.Agency == null ? null : (int?)user.Profile.Agency.AgencyId,
                    contractorId = user.Profile.ContractorCompany == null ? null : (int?)user.Profile.ContractorCompany.ContractorId,
                    emailAddress = user.Profile.EmailAddress,
                    user.Profile.CrashReportAccess,
                    appNm = _applicationName
                });

            // MERGE INTO USER_CNTY
            foreach (var county in user.Profile.ViewableCounties)
            {
                MergeUserCounty(user.UserName, county);
            }

            // MERGE INTO USER_AGREEMENT
            foreach (var agreement in user.Profile.Agreements)
            {
                MergeUserAgreement(user.UserName, agreement);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (user.Profile == null)
            {
                // return IdentityResult.Failed(new IdentityError() { Description = "User profile cannot be null." });
                throw new Exception("User profile cannot be null."); // temporary
            }

            // UPDATE S4_USER
            var updateTxt = @"UPDATE S4_USER
                SET FIRST_NM = :firstName,
                    LAST_NM = :lastName,
                    NAME_SUFFIX = :suffixName,
                    FORCE_PASSWORD_CHANGE = :forcePasswordChange,
                    TIME_LIMITED_ACCOUNT_CD = :timeLimitedAccount,
                    ACCOUNT_START_DT = :accountStartDate,
                    ACCOUNT_EXPIRATION_DT = :accountExpirationDate,
                    AGNCY_ID = :agencyId,
                    CONTRACTOR_ID = :contractorId,
                    EMAIL = :emailAddress,
                    CAN_VIEW = :crashReportAccess,
                    MODIFIED_BY = :modifiedBy,
                    MODIFIED_DT = :modifiedDate
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName";

            await _connection.ExecuteAsync(updateTxt, new {
                user.Profile.FirstName,
                user.Profile.LastName,
                user.Profile.SuffixName,
                forcePasswordChange = user.Profile.ForcePasswordChange ? "Y" : "N",
                timeLimitedAccount = user.Profile.TimeLimitedAccount ? "Y" : "N",
                user.Profile.AccountStartDate,
                user.Profile.AccountExpirationDate,
                agencyId = user.Profile.Agency == null ? null : (int?)user.Profile.Agency.AgencyId,
                contractorId = user.Profile.ContractorCompany == null ? null : (int?)user.Profile.ContractorCompany.ContractorId,
                user.Profile.EmailAddress,
                user.Profile.CrashReportAccess,
                modifiedBy = "TBD",
                modifiedDate = DateTime.Now,
                appName = _applicationName,
                user.UserName
            });

            // MERGE INTO USER_CNTY
            foreach (var county in user.Profile.ViewableCounties)
            {
                MergeUserCounty(user.UserName, county);
            }

            // DELETE USER_CNTY
            var deleteText = @"DELETE FROM user_cnty
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName";
            if (user.Profile.ViewableCounties.Count > 0)
            {
                deleteText += " AND cnty_cd NOT IN :countyCodes";
            }
            _connection.Execute(deleteText, new {
                appName = _applicationName,
                user.UserName,
                countyCodes = user.Profile.ViewableCounties.Select(c => c.CountyCode)
            });

            // MERGE INTO USER_AGREEMENT
            foreach (var agreement in user.Profile.Agreements)
            {
                MergeUserAgreement(user.UserName, agreement);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var @params = new { appName = _applicationName, user.UserName };

            // DELETE FROM USER_CNTY
            var deleteTxt = @"DELETE FROM user_cnty WHERE application_nm = :appName AND user_nm = :userName";
            await _connection.ExecuteAsync(deleteTxt, @params);

            // DELETE FROM USER_AGREEMENT
            deleteTxt = @"DELETE FROM user_agreement WHERE application_nm = :appName AND user_nm = :userName";
            await _connection.ExecuteAsync(deleteTxt, @params);

            // DELETE FROM S4_USER
            deleteTxt = @"DELETE FROM s4_user WHERE application_nm = :appName AND user_nm = :userName";
            await _connection.ExecuteAsync(deleteTxt, @params);

            return IdentityResult.Success;
        }

        private IdentityResult MergeUserAgreement(string userName, UserAgreement agreement)
        {
            var cmdText = @"MERGE INTO user_agreement ua
                USING (
                  SELECT
                    :appName AS application_nm,
                    :userName AS user_nm,
                    :agreementName AS agreement_nm,
                    :signedDate AS agreement_signed_dt,
                    :expirationDate AS agreement_expiration_dt
                  FROM dual
                ) x
                ON (x.application_nm = ua.application_nm
                AND x.user_nm = ua.user_nm
                AND x.agreement_nm = ua.agreement_nm)
                WHEN MATCHED THEN UPDATE SET
                  ua.agreement_signed_dt = x.agreement_signed_dt,
                  ua.agreement_expiration_dt = x.agreement_expiration_dt
                WHEN NOT MATCHED THEN INSERT
                  (ua.application_nm, ua.user_nm, ua.agreement_nm, ua.agreement_signed_dt, ua.agreement_expiration_dt)
                  VALUES (x.application_nm, x.user_nm, x.agreement_nm, x.agreement_signed_dt, x.agreement_expiration_dt)";
            _connection.Execute(cmdText, new {
                appName = _applicationName,
                userName,
                agreement.AgreementName,
                agreement.SignedDate,
                agreement.ExpirationDate
            });
            return IdentityResult.Success;
        }

        private IdentityResult MergeUserCounty(string userName, UserCounty county)
        {
            var cmdText = @"MERGE INTO user_cnty uc
                USING (
                  SELECT
                    :appName AS application_nm,
                    :userName AS user_nm,
                    :countyCode AS cnty_cd,
                    :canView AS can_view,
                    :canEdit AS can_edit
                  FROM dual
                ) x
                ON (x.application_nm = uc.application_nm
                AND x.user_nm = uc.user_nm
                AND x.cnty_cd = uc.cnty_cd)
                WHEN MATCHED THEN UPDATE SET
                  uc.can_view = x.can_view,
                  uc.can_edit = x.can_edit,
                  uc.modified_by = :currentUserName,
                  uc.modified_dt = :currentTime
                WHEN NOT MATCHED THEN INSERT
                  (uc.application_nm, uc.user_nm, uc.cnty_cd, uc.can_view, uc.can_edit, uc.created_by, uc.created_dt)
                  VALUES (x.application_nm, x.user_nm, x.cnty_cd, x.can_view, x.can_edit, :currentUserName, :currentTime)";
            _connection.Execute(cmdText, new
            {
                appName = _applicationName,
                userName,
                county.CountyCode,
                canView = county.CrashReportAccess,
                canEdit = county.CanEdit ? "Y" : "N",
                currentUserName = "TBD",
                currentTime = DateTime.Now
            });
            return IdentityResult.Success;
        }
    }
}
