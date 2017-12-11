using System;

namespace S4Analytics.Models
{
    public class UserCounty
    {
        #region Public Properties

        public string CountyName { get; set; }
        public int CountyCode { get; set; }
       // public CrashReportAccess CrashReportAccess { get; set; }
        public bool CanEdit { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        #endregion
    }
}
