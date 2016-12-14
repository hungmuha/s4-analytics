using System;


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

    public enum NewUserRequestType
    {
        Unknown = 0,
        FlPublicAgencyEmployee = 1,
        FlPublicAgencyMgr = 2,
        NonFlPublicAgencyEmployee = 3,
        Other = 4
    }

    public enum NewUserRequestStatus
    {
        Unknown = 0,
        NewUser = 1,
        NewAgencyAndUser = 2,
        NewConsultant = 3,
        NewContractorAndConsultant = 4,
        CreateAgencyAndUser = 5,
        VerifyEmail = 6,
        VerifyContract = 7,
        VerifyNonFLUser = 8,
        NewNonFLUser = 9,
        Completed = 10,
        Rejected = 11
    }

    public class NewUserRequest
    {
        public int RequestNbr { get; set; }
        public DateTime RequestDt { get; set; }
        public string RequestDesc { get; set; }
        public NewUserRequestType RequestType { get; set; }
        public NewUserRequestStatus RequestStatus { get; set; }
        public DateTime UserCreatedDt { get; set; }
        public int AgncyId { get; set; }
        public AgencyType? NewAgncyTypeCd { get; set; }
        public string NewAgncyNm { get; set; }
        public string NewAgncyEmailDomain { get; set; }
        public string RequestorFirstNm { get; set; }
        public string RequestorLastNm { get; set; }
        public string RequestorSuffixNm { get; set; }
        public string RequestorEmail { get; set; }
        public int ContractorId { get; set; }
        public string NewContractorNm { get; set; }
        public string NewContractorEmailDomain { get; set; }
        public string ConsultantFirstNm { get; set; }
        public string ConsultantLastNm { get; set; }
        public string ConsultantSuffixNm { get; set; }
        public string ConsultantEmail { get; set; }
        public string AccessReasonTx { get; set; }
        public DateTime? ContractStartDt { get; set; }
        public DateTime? ContractEndDt { get; set; }
        public string UserId { get; set; }
        public char WarnRequestorEmailCd { get; set; }
        public char WarnUserEmailCd { get; set; }
        public char WarnUserAgncyEmailCd { get; set; }
        public char WarnUserVendorEmailCd { get; set; }
        public string AdminComment { get; set; }
    }
}
