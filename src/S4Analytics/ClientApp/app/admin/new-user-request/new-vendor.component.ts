import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-vendor-component',
    templateUrl: './new-vendor.component.html'
})

export class NewVendorComponent {

    constructor(public state: NewUserRequestStateService) {

    }

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

    openContractViewer() {
        let contractPdfFileNm = this.state.selectedRequest.contractPdfNm;
        this.state.contractViewerWindow = window.open(`admin/new-user-request/contract-pdf/${contractPdfFileNm}`,
            '_blank', 'width=400,height=200');
    }


}
