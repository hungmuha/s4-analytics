import { Component } from '@angular/core';
import { NewUserRequestStateService, NewUserRequestStatus } from './shared';
import { DatePipe } from '@angular/common';
import { IdentityService, S4IdentityUser } from '../../shared';


@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {

    contractEndDateStr: string;

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

        let transformDt = this.datePipe.transform(this.state.selectedRequest.contractEndDt, 'M/d/yyyy');
        this.contractEndDateStr = (transformDt) ? transformDt : '';
    }

    hideRequestorWarning(): boolean {
        return !this.state.selectedRequest.warnRequestorEmailCd;
    }

    hideConsultantWarning(): boolean {
        return !this.state.selectedRequest.warnConsultantEmailCd && !this.state.selectedRequest.warnDuplicateEmailCd;
    }

    isValidDate(dateStr: string) {
        let dateRegex = this.state.dateRegex;
        return dateRegex.test(dateStr);
    }

    // Check if end of contract date is after start of contract date
    isValidDateRange(): boolean {
        if (!this.state.selectedRequest.contractStartDt) { return false;}

        let startDt = this.state.selectedRequest.contractStartDt;
        let endDt = new Date(this.contractEndDateStr);

        return (endDt >= startDt);
    }

    isContractEndDateReadOnly(): boolean {
        let currentUser = this.identityService.currentUser as S4IdentityUser;
        let request = this.state.selectedRequest;

        return request.requestStatus === NewUserRequestStatus.Completed
            || request.requestStatus === NewUserRequestStatus.Rejected
            || currentUser.roles.indexOf('global admin') > -1;
    }

    isNewVendor(): boolean {
        return this.state.selectedRequest.initialRequestStatus === NewUserRequestStatus.NewVendor;
    }

    contractEndDateStrChanged(dateStr: string) {
        this.contractEndDateStr = dateStr;
        this.state.currentActionForm.valid = this.isComponentValid();
        if (this.isComponentValid()) {
            this.state.currentRequestActionResults.contractEndDt = new Date(this.contractEndDateStr);
        }
    }

    isComponentValid(): boolean {
        return this.isValidDate(this.contractEndDateStr) && this.isValidDateRange();
    }
}
