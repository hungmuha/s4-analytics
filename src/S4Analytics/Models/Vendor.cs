using System;

namespace S4Analytics.Models
{
    public class Vendor
    {
        #region Public Properties

        public string VendorName { get; set; }
        public int VendorId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string EmailDomain { get; set; }

        #endregion
        public Vendor()
        {
        }

        public Vendor(string vendorName, int vendorId)
        {
            VendorName = vendorName;
            VendorId = vendorId;
        }
    }
}