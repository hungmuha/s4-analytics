import { NewUserRequestType, NewUserRequestStatus } from './new-user-request-enum';

export class NewUserRequest {
    requestNbr: number;
    requestDt: Date;
    requestDesc: string;
    requestType: NewUserRequestType;
    requestStatus: NewUserRequestStatus;
    initialRequestStatus: NewUserRequestStatus;
    userCreatedDt?: Date;
    agncyId: number;
    agncyNm: string;
    agncyEmailDomain: string;
    requestorFirstNm: string;
    requestorLastNm:  string;
    requestorSuffixNm: string;
    requestorEmail: string;
    vendorId: number;
    vendorName: string;
    vendorEmailDomain: string;
    consultantFirstNm: string;
    consultantLastNm:  string;
    consultantSuffixNm: string;
    consultantEmail: string;
    accessReasonTx:  string;
    contractStartDt?: Date;
    contractEndDt?: Date;
    userId: string;
    warnRequestorEmailCd: boolean;
    warnConsultantEmailCd: boolean;
    warnDuplicateEmailCd: boolean;
    adminComment: string;
    accessBefore70Days: boolean = false;
    contractPdfNm: string;
    agencyHasAdmin: boolean;
    handledBy: string;

    constructor(data: NewUserRequest) {
        // merge data from the api
        Object.assign(this, data);
        // rest api represents dates as strings at runtime; convert
        this.userCreatedDt = (this.userCreatedDt == undefined) ? undefined : new Date(this.userCreatedDt);
        this.contractStartDt = (this.contractStartDt == undefined) ? undefined : new Date(this.contractStartDt);
        this.contractEndDt = (this.contractEndDt == undefined) ? undefined : new Date(this.contractEndDt);
    }
}
