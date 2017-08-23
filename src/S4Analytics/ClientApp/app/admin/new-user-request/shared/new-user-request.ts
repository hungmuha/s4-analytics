import { NewUserRequestType, NewUserRequestStatus } from './new-user-request-enum';

export class NewUserRequest {
    requestNbr: number;
    requestDt: Date;
    requestDesc: string;
    requestType: NewUserRequestType;
    requestStatus: NewUserRequestStatus;
    initialRequestStatus: NewUserRequestStatus;
    userCreatedDt: Date;
    agncyNm: string;
    agncyEmailDomain: string;
    requestorFirstNm: string;
    requestorLastNm:  string;
    requestorSuffixNm: string;
    requestorEmail: string;
    vendorName: string;
    vendorEmailDomain: string;
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
    warnDuplicateEmailCd: boolean;
    adminComment: string;
    accessBefore70Days: boolean = false;
    contractPdfNm: string;
}