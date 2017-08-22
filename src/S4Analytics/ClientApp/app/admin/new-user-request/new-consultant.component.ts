import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {

    constructor(public state: NewUserRequestStateService) { }

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
    }

    hideRequestorWarning(): boolean {
        return !this.state.selectedRequest.warnRequestorEmailCd;
    }


    hideConsultantWarning(): boolean {
        return !this.state.selectedRequest.warnConsultantEmailCd && !this.state.selectedRequest.warnDuplicateEmailCd;
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
