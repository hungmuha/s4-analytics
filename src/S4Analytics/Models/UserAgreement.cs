using System;

namespace S4Analytics.Models
{
    public class AgreementNames
    {
        public const string UserAgreement = "User Agreement";
        public const string UserManagerAgreement = "User Manager Agreement";
    }

    public class UserAgreement
    {
        public string AgreementName { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsExpired
        {
            get
            {
                return ExpirationDate == null || ExpirationDate <= DateTime.Now;
            }
        }
    }
}
