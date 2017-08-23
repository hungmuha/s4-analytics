using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public enum AgencyType
    {
        NonLEA = 0,
        FHP = 1,
        PoliceDept = 2,
        SheriffsOffice = 3,
        Other = 4
    }

    public class Agency
    {
        #region Public Properties

        public string AgencyName { get; set; }
        public AgencyType AgencyTypeCd { get; set; }
        public int AgencyId { get; set; }
        public int ParentAgencyId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<UserCounty> DefaultViewableCounties { get; set; }
        public CrashReportAccess CrashReportAccess { get; set; }
        public string EmailDomain { get; set; }

        public bool IsLawEnforcementAgency
        {
            get
            {
                switch (AgencyTypeCd)
                {
                    case AgencyType.FHP:
                    case AgencyType.PoliceDept:
                    case AgencyType.SheriffsOffice:
                        return true;
                    default:
                        return false;
                }
            }
            set { }
        }

        #endregion

        // Default constructor
        public Agency()
        {
            //TODO
            IsActive = true;
            CrashReportAccess = CrashReportAccess.Unknown;
            DefaultViewableCounties = new List<UserCounty>();

        }

        public List<UserCounty> GetDefaultEditableCounties()
        {
            var _editableCounties = new List<UserCounty>();
            if (DefaultViewableCounties != null)
            {
                _editableCounties.AddRange(DefaultViewableCounties.Where(cnty => cnty.CanEdit));
            }
            return _editableCounties;
        }
    }


}