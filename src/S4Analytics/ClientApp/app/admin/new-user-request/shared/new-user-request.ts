import { AgencyType, NewUserRequestType, NewUserRequestStatus } from './new-user-request-enum';

export class NewUserRequest {
    requestNbr: number;
    requestDt: Date;
    requestDesc: string;
    requestType: NewUserRequestType;
    requestStatus: NewUserRequestStatus;
    userCreatedDt: Date;
    agncyNm: string;
    newAgncyTypeCd: AgencyType;
    newAgncyNm: string;
    newAgncyEmailDomain: string;
    requestorFirstNm: string;
    requestorLastNm:  string;
    requestorSuffixNm: string;
    requestorEmail: string;
    contractorNm: string;
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
    warnRequestorEmailCd: boolean;
    warnConsultantEmailCd: boolean;
    adminComment: string;
    accessBefore70Days: boolean = false;

}