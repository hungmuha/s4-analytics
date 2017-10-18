﻿import { NewUserRequestType, NewUserRequestStatus } from './new-user-request-enum';

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

    constructor(data: NewUserRequest) {
        this.requestNbr = data.requestNbr;
        this.requestDt = data.requestDt;
        this.requestDesc = data.requestDesc;
        this.requestType = data.requestType;
        this.requestStatus = data.requestStatus;
        this.initialRequestStatus = data.initialRequestStatus;
        this.userCreatedDt = (data.userCreatedDt == undefined) ? undefined : new Date(data.userCreatedDt);
        this.agncyId = data.agncyId;
        this.agncyNm = data.agncyNm;
        this.agncyEmailDomain = data.agncyEmailDomain;
        this.requestorFirstNm = data.requestorFirstNm;
        this.requestorLastNm = data.requestorLastNm;
        this.requestorSuffixNm = data.requestorSuffixNm;
        this.requestorEmail = data.requestorEmail;
        this.vendorName = data.vendorName;
        this.vendorEmailDomain = data.vendorEmailDomain;
        this.consultantFirstNm = data.consultantFirstNm;
        this.consultantLastNm = data.consultantLastNm;
        this.consultantSuffixNm = data.consultantSuffixNm;
        this.consultantEmail = data.consultantEmail;
        this.accessReasonTx = data.accessReasonTx;
        this.contractStartDt = (data.contractStartDt == undefined) ? undefined : new Date(data.contractStartDt);
        this.contractEndDt = (data.contractEndDt == undefined) ? undefined : new Date(data.contractEndDt);
        this.userId = data.userId;
        this.warnRequestorEmailCd = data.warnRequestorEmailCd;
        this.warnConsultantEmailCd = data.warnConsultantEmailCd;
        this.warnDuplicateEmailCd = data.warnDuplicateEmailCd;
        this.adminComment = data.adminComment;
        this.accessBefore70Days = data.accessBefore70Days;
        this.contractPdfNm = data.contractPdfNm;
    }
}