export enum QueueFilter {
    All,
    Pending,
    Completed,
    Rejected
}

export enum QueueColumn {
    ReqNbr,
    ReqDt,
    ReqType,
    Requestor,
    ReqAgncy,
    ReqStatus,
    AcctCreated,
    Comment
}

export enum AgencyType {
    NonLEA = 0,
    FHP = 1,
    PoliceDept = 2,
    SheriffsOffice = 3,
    Other = 4
}

export enum NewUserRequestType {
    Unknown = 0,
    FlPublicAgencyEmployee = 1,
    FlPublicAgencyMgr = 2,
    NonFlPublicAgencyEmployee = 3,
    Other = 4
}

export enum NewUserRequestStatus {
    Unknown = 0,
    NewUser = 1,
    NewAgency = 2,
    NewConsultant = 3,
    NewVendor = 4,
    CreateAgency = 5,
    Completed = 10,
    Rejected = 11
}