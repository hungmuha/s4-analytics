import { Component } from '@angular/core';
import { NewUserRequestStateService, NewUserRequestStatus } from './shared';
import { DatePipe } from '@angular/common';
import { IdentityService, S4IdentityUser } from '../../shared';


@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {

    constructor(
        public state: NewUserRequestStateService,
        private datePipe: DatePipe,
        private identityService: IdentityService) { }

    get contractViewerUrl() {
        return `admin/new-user-request/contract-pdf/${this.state.selectedRequest.contractPdfNm}`;
    }

    ngOnInit() {
        this.state.requestorWarningMessages = [];
        this.state.consultantWarningMessages = [];

        if (this.state.selectedRequest.warnConsultantEmailCd) {
            this.state.consultantWarningMessages.push('Consultant/Vendor email domain mismatch');
        }

        if (this.state.selectedRequest.warnDuplicateEmailCd) {
            this.state.consultantWarningMessages.push('An account with email already exists');
        }

        if (this.state.selectedRequest.warnRequestorEmailCd) {
            this.state.requestorWarningMessages.push('Requestor/Agency email domain mismatch');
        }

        if (this.state.selectedRequest.contractEndDt !== undefined) {
            let transformDt = this.datePipe.transform(this.state.selectedRequest.contractEndDt, 'M/d/yyyy');
            this.state.currentRequestActionResults.contractEndDt = (transformDt) ? transformDt : '';
        }
    }

    hideRequestorWarning(): boolean {
        return !this.state.selectedRequest.warnRequestorEmailCd;
    }

    hideConsultantWarning(): boolean {
        return !this.state.selectedRequest.warnConsultantEmailCd && !this.state.selectedRequest.warnDuplicateEmailCd;
    }

    invalidDateRange(): boolean {
        let startDt = new Date(this.state.selectedRequest.contractStartDt);
        let endDt = new Date(this.state.currentRequestActionResults.contractEndDt);

        this.state.currentActionForm.valid = (endDt >= startDt);
        return (endDt < startDt);
    }

    showInvalidDateMsg(endDtInvalid: boolean): boolean {
        this.state.currentActionForm.valid = !endDtInvalid;
        return endDtInvalid;
    }

    contractEndDateReadOnly(): boolean {
        let currentUser = this.identityService.currentUser as S4IdentityUser;
        let request = this.state.selectedRequest;

        return request.requestStatus === NewUserRequestStatus.Completed
            || request.requestStatus === NewUserRequestStatus.Rejected
            || currentUser.roles.indexOf('global admin') > -1;
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            this.state.currentRequestActionResults.accessBefore70Days = false;
        }
    }
}
