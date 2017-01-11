import { AgencyType, NewUserRequestType, NewUserRequestStatus } from './new-user-request-enum';

export class NewUserRequest {
    requestNbr: number;
    requestDt: Date;
    requestDesc: string;
    requestType: NewUserRequestType;
    requestStatus: NewUserRequestStatus;
    userCreatedDt: Date;
    agncyId: number;
    newAgncyTypeCd: AgencyType;
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
    warnRequestorEmailCd: string;
    warnConsultantEmailCd: string;
    adminComment: string;

}