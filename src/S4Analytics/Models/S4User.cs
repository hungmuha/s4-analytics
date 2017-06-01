using System;
using System.Collections.Generic;
using System.Linq;
using Lib.Identity;

namespace S4Analytics.Models
{
    public enum CrashReportAccess
    {
        Unknown = 0,
        Within60Days = 1,
        After60Days = 2,
        NoAccess = 3
    }

    public class RoleNames
    {
        public const string GlobalAdmin = "Global Admin";
        public const string AgencyAdmin = "Agency Admin";
        public const string HSMVAdmin = "HSMV Admin";
        public const string FDOTAdmin = "FDOT Admin";
        public const string Editor = "Editor";
        public const string Guest = "Guest";
        public const string User = "User";
        public const string PbcatEditor = "PBCAT Editor";
    }

    public class StickySettings
    {
        public string AgencyScope { get; set; }
        public string CrashType { get; set; }
        public string CityScope { get; set; }
        public string CountyScope { get; set; }
        public string DotDistrictScope { get; set; }
        public string FhpTroopScope { get; set; }
        public string GeographicExtent { get; set; }
        public string IntrsectOffsetDistFt { get; set; }
        public string MpoTpoScope { get; set; }

    }

    public class S4User
    {
        #region Private fields

        private Dictionary<string, List<UserRole>> _allUserRoles;
        private Dictionary<string, List<UserCounty>> _allUserCounties;

        #endregion

        #region Public Properties

        public S4IdentityUser IdentityUser { get; set; }
        public DateTime? AccountExpirationDate { get; set; }
        public DateTime? AccountStartDate { get; set; }
        public bool Active { get; set; }
        public DateTime? AdminAgreementExpirationDate { get; set; }
        public DateTime? AdminAgreementSignedDate { get; set; }
        public Agency Agency { get; set; }
        public Contractor ContractorCompany { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string SuffixName { get; set; }
        public bool ForcePasswordChange { get; set; }
        public bool IsAdminAgreementExpired { get { return (AdminAgreementExpirationDate <= DateTime.Now); } set { } }
        public bool IsAgencyAdmin { get { return UserRoles.Any(t => t.RoleName == RoleNames.AgencyAdmin); } set { } }
        public bool IsEditor { get { return UserRoles.Any(t => t.RoleName == RoleNames.Editor); } set { } }
        public bool IsPbcatEditor { get { return UserRoles.Any(t => t.RoleName == RoleNames.PbcatEditor); } set { } }
        public bool IsGlobalAdmin { get { return UserRoles.Any(t => t.RoleName == RoleNames.GlobalAdmin); } set { } }
        public bool IsGuest { get { return UserRoles.Any(t => t.RoleName == RoleNames.Guest); } set { } }
        public bool IsTimeLimitedAccountExpired { get { return (TimeLimitedAccount && AccountExpirationDate <= DateTime.Now); } set { } }
        public bool IsUserAgreementExpired { get { return (UserAgreementExpirationDate <= DateTime.Now); } set { } }
        public string LastName { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool ReadOnly { get; set; }
        public bool TimeLimitedAccount { get; set; }
        public DateTime? UserAgreementSignedDate { get; set; }
        public DateTime? UserAgreementExpirationDate { get; set; }
        public string UserName { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public StickySettings StickySettings { get; set; }
        public List<UserCounty> ViewableCounties { get; set; }
        public CrashReportAccess CrashReportAccess { get; set; }

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public S4User()
        {
            AccountExpirationDate = null;
            AccountStartDate = null;
            Active = true;
            AdminAgreementSignedDate = null;
            AdminAgreementExpirationDate = null;
            Agency = null;
            ContractorCompany = null;
            CreatedBy = string.Empty;
            CreatedDate = null;
            EmailAddress = string.Empty;
            FirstName = string.Empty;
            ForcePasswordChange = true;
            LastName = string.Empty;
            ModifiedBy = string.Empty;
            ModifiedDate = null;
            ReadOnly = true;
            SuffixName = string.Empty;
            TimeLimitedAccount = false;
            UserAgreementSignedDate = null;
            UserAgreementExpirationDate = null;
            UserName = string.Empty;
            UserRoles = new List<UserRole>();
            ViewableCounties = new List<UserCounty>();
            CrashReportAccess = CrashReportAccess.Unknown;

        }

        public List<UserCounty> GetEditableCounties()
        {
            var _editableCounties = new List<UserCounty>();
            if (ViewableCounties != null)
            {
                _editableCounties.AddRange(ViewableCounties.Where(cnty => cnty.CanEdit));
            }
            return _editableCounties;
        }
    }
}
