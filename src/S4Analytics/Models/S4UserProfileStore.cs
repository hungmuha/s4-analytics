using Dapper;
using Lib.Identity;
using Lib.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<S4UserProfile> FindProfileForUserAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            var selectText = @"SELECT
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
                FROM
                  S4_USER
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName";

            var profile = await _conn.QueryFirstOrDefaultAsync<S4UserProfile>(selectText, new { appName = _applicationName, user.UserName });

            if (profile != null)
            {
                profile.ViewableCounties = GetCountiesForUser(user.UserName);
                profile.Agency = GetAgencyForUser(user.UserName);
                profile.ContractorCompany = GetContractorForUser(user.UserName);
                profile.Agreements = GetAgreementsForUser(user.UserName);
            }

            return profile;
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
			(APPLICATION_NM, USER_NM, CNTY_CD, CAN_VIEW, CAN_EDIT, CREATED_BY, CREATED_DT)
			VALUES (:appNm, :userName, :cntyCd, :canView, :canEdit, :createdBy, :createdDate)";

            foreach (UserCounty cnty in user.Profile.ViewableCounties)
            {
                await _conn.ExecuteAsync(insertTxt, new
                {
                    user.UserName,
                    cnty.CountyCode,
                    canView = cnty.CrashReportAccess,
                    canEdit = cnty.CanEdit ? "Y" : "N",
                    cnty.CreatedBy,
                    cnty.CreatedDate,
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

            // INSERT/UPDATE USER_CNTY
            foreach (var county in user.Profile.ViewableCounties)
            {
                UpdateUserCounty(user.UserName, county);
            }

            // DELETE USER_CNTY
            var deleteText = @"DELETE FROM user_cnty
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName";
            if (user.Profile.ViewableCounties.Count > 0)
            {
                deleteText += " AND cnty_cd NOT IN :countyCodes";
            }
            _conn.Execute(deleteText, new {
                appName = _applicationName,
                user.UserName,
                countyCodes = user.Profile.ViewableCounties.Select(c => c.CountyCode)
            });

            // INSERT/UPDATE USER_AGREEMENT
            foreach (var agreement in user.Profile.Agreements)
            {
                UpdateUserAgreement(user.UserName, agreement);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteProfileAsync(S4IdentityUser<S4UserProfile> user, CancellationToken cancellationToken)
        {
            var @params = new { appName = _applicationName, user.UserName };

            // DELETE FROM USER_CNTY
            var deleteTxt = @"DELETE FROM user_cnty WHERE application_nm = :appName AND user_nm = :userName";
            await _conn.ExecuteAsync(deleteTxt, @params);

            // DELETE FROM USER_AGREEMENT
            deleteTxt = @"DELETE FROM user_agreement WHERE application_nm = :appName AND user_nm = :userName";
            await _conn.ExecuteAsync(deleteTxt, @params);

            // DELETE FROM S4_USER
            deleteTxt = @"DELETE FROM s4_user WHERE application_nm = :appName AND user_nm = :userName";
            await _conn.ExecuteAsync(deleteTxt, @params);

            return IdentityResult.Success;
        }

        private Agency GetAgencyForUser(string userName)
        {
            var selectTxt = @"SELECT
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
                AND u.user_nm = :userName";

            var agency = _conn.QueryFirstOrDefault<Agency>(selectTxt, new { appName = _applicationName, userName });
            return agency;
        }

        private Contractor GetContractorForUser(string userName)
        {
            var selectTxt = @"SELECT
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
                AND u.user_nm = :userName";

            var contractor = _conn.QueryFirstOrDefault<Contractor>(selectTxt, new { appName = _applicationName, userName });
            return contractor;
        }

        private List<UserCounty> GetCountiesForUser(string userName)
        {
            var selectTxt = @"SELECT
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
                AND USER_NM = :userName";

            var counties = _conn.Query<UserCounty>(selectTxt, new { appName = _applicationName, userName }).ToList();
            return counties;
        }

        private List<UserAgreement> GetAgreementsForUser(string userName)
        {
            var selectText = @"SELECT
                  AGREEMENT_NM AS AgreementName,
                  AGREEMENT_SIGNED_DT AS AgreementSignedDate,
                  AGREEMENT_EXPIRATION_DT AS AgreementExpirationDate
                FROM
                  USER_AGREEMENT
                WHERE APPLICATION_NM = :appName
                AND USER_NM = :userName";

            var agreements = _conn.Query<UserAgreement>(selectText, new { appName = _applicationName, userName }).ToList();
            return agreements;
        }

        private IdentityResult UpdateUserAgreement(string userName, UserAgreement agreement)
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
            _conn.Execute(cmdText, new {
                appName = _applicationName,
                userName,
                agreement.AgreementName,
                agreement.SignedDate,
                agreement.ExpirationDate
            });
            return IdentityResult.Success;
        }

        private IdentityResult UpdateUserCounty(string userName, UserCounty county)
        {
            var cmdText = @"MERGE INTO user_cnty uc
                USING (
                  SELECT
                    :appName AS application_nm,
                    :userName AS user_nm,
                    :countyCode AS cnty_cd,
                    :canView AS can_view,
                    :canEdit AS can_edit,
                    :createdBy AS created_by,
                    :createdDate AS created_dt,
                    :modifiedBy AS modified_by,
                    :modifiedDate AS modified_dt
                  FROM dual
                ) x
                ON (x.application_nm = uc.application_nm
                AND x.user_nm = uc.user_nm
                AND x.cnty_cd = uc.cnty_cd)
                WHEN MATCHED THEN UPDATE SET
                  uc.can_view = x.can_view,
                  uc.can_edit = x.can_edit,
                  uc.created_by = x.created_by,
                  uc.created_dt = x.created_dt,
                  uc.modified_by = x.modified_by,
                  uc.modified_dt = x.modified_dt
                WHEN NOT MATCHED THEN INSERT
                  (uc.application_nm, uc.user_nm, uc.cnty_cd, uc.can_view, uc.can_edit, uc.created_by, uc.created_dt, uc.modified_by, uc.modified_dt)
                  VALUES (x.application_nm, x.user_nm, x.cnty_cd, x.can_view, x.can_edit, x.created_by, x.created_dt, x.modified_by, x.modified_dt)";
            _conn.Execute(cmdText, new
            {
                appName = _applicationName,
                userName,
                county.CountyCode,
                canView = county.CrashReportAccess,
                canEdit = county.CanEdit ? "Y" : "N",
                county.CreatedBy,
                county.CreatedDate,
                county.ModifiedBy,
                county.ModifiedDate
            });
            return IdentityResult.Success;
        }
    }
}
