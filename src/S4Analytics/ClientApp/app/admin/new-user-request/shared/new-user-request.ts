export class NewUserRequest {
    requestNbr: number;
    requestDt: Date;
    requestDesc: string;
    requestType: number;
    requestStatus: number;
    userCreatedDt: Date;
    agncyId: number;
    newAgncyTypeCd: number;
    newAgncyNm: string;
    newAgncyEmailDomain: string;
    requestorFirstNm: string;
    requestorLastNm:  string;
    requestorSuffixNm: string;
    requestorEmail: string;
    contractorId: number;
    newContractorNm: string;
    newContractorEmailDomain: string;
    consultantFirstNm: string;
    consultantLastNm:  string;
    consultantSuffixNm: string;
    consultantEmail: string;
    accessReasonTx:  string;
    contractStartDt: Date;
    contractEndDt: Date;
    userId: string;
    warnRequestorEmail: string;
    warnUserEmailCd: string;
    warnUserAgncyEmailCd: string;
    warnUserVendorEmailCd: string;
    adminComment: string;

}