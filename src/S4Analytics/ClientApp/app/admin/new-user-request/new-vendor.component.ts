import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-vendor-component',
    templateUrl: './new-vendor.component.html'
})

export class NewVendorComponent {

    get contractViewerUrl() {
        return `admin/new-user-request/contract-pdf/${this.state.selectedRequest.contractPdfNm}`;
    }

    constructor(public state: NewUserRequestStateService) { }

    ngOnInit() {
        this.state.requestorWarningMessages = [];

        if (this.state.selectedRequest.warnRequestorEmailCd) {
            this.state.requestorWarningMessages.push('Requestor/Agency email domain mismatch');
        }
    }

    hideRequestorWarning(): boolean {
        return !this.state.selectedRequest.warnRequestorEmailCd;
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }
}
