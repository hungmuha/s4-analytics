using System;

namespace S4Analytics.Models
{
    public class UserRole
    {
        public string RoleName { get; set; }
        public DateTime AgreementExpirationDt { get; set; }
        public DateTime AgreementSignedDt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}
