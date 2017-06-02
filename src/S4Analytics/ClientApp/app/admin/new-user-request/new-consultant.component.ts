import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {

    constructor(public state: NewUserRequestStateService
) {

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

