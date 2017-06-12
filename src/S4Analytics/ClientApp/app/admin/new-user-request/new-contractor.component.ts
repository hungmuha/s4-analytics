import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-contractor-component',
    templateUrl: './new-contractor.component.html'
})

export class NewContractorComponent {

    constructor(public state: NewUserRequestStateService) {

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
