﻿using System;
using Lib.Identity;
using Lib.Identity.Models;

namespace S4Analytics.Models
{
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
        NewAgency = 2,
        NewConsultant = 3,
        NewVendor = 4,
        CreateAgency = 5,
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
        public NewUserRequestStatus InitialRequestStatus { get; set; }
        public DateTime? UserCreatedDt { get; set; }
        public int AgncyId { get; set; }
        public string AgncyNm { get; set; }
        public string AgncyEmailDomain { get; set; }
        public string RequestorFirstNm { get; set; }
        public string RequestorLastNm { get; set; }
        public string RequestorSuffixNm { get; set; }
        public string RequestorEmail { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorEmailDomain { get; set; }
        public string ConsultantFirstNm { get; set; }
        public string ConsultantLastNm { get; set; }
        public string ConsultantSuffixNm { get; set; }
        public string ConsultantEmail { get; set; }
        public string AccessReasonTx { get; set; }
        public DateTime? ContractStartDt { get; set; }
        public DateTime? ContractEndDt { get; set; }
        public string UserId { get; set; }
        public bool WarnRequestorEmailCd { get; set; }
        public bool WarnConsultantEmailCd { get; set; }
        public bool WarnDuplicateEmailCd { get; set; }
        public string AdminComment { get; set; }
        public bool AccessBefore70Days { get; set; }
        public string HandledBy { get; set; }
        public bool UserManagerCd { get; set; }
        public string ContractPdfNm { get; set; }
        public bool AgencyHasAdmin { get; set; }
    }
}
