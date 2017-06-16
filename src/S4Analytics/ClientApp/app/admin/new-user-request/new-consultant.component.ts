import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {

    constructor(public state: NewUserRequestStateService) {

    }

    ngOnInit() {
        this.state.warningMessages = [];

        if (this.state.selectedRequest.warnConsultantEmailCd) {
            this.state.warningMessages.push('Consultant/Vendor email domain mismatch');
        }

        if (this.state.selectedRequest.warnDuplicateEmailCd) {
            this.state.warningMessages.push('An account with email already exists');
        }

        if (this.state.selectedRequest.warnRequestorEmailCd) {
            this.state.warningMessages.push('Requestor/Agency email domain mismatch');
        }
    }


    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            this.state.currentRequestActionResults.accessBefore70Days = false;
        }
    }

    openContractViewer() {
        let contractPdfFileNm = this.state.selectedRequest.contractPdfNm;
        this.state.contractViewerWindow = window.open(`admin/new-user-request/contract-pdf/${contractPdfFileNm}`,
            '_blank', 'width=400,height=200');
    }



}

