using System;

namespace S4Analytics.Models
{
    public class Contractor
    {
        #region Public Properties

        public string ContractorName { get; set; }
        public int ContractorId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string EmailDomain { get; set; }

        #endregion
        public Contractor()
        {
        }

        public Contractor(string contractorNm, int contractorId)
        {
            ContractorName = contractorNm;
            ContractorId = contractorId;
        }
    }
}