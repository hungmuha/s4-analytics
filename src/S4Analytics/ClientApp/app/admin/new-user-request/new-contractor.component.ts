import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';
import * as moment from 'moment';

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
        let contractPdfFileNm = this.getContractPdfFileName();
        this.state.contractViewerWindow = window.open(`api/admin/new-user-request/contract-pdf/${contractPdfFileNm}`,
            '_blank', 'width=400,height=200');
    }

    private getContractPdfFileName(): string {
        let request = this.state.selectedRequest;
        let requestDate = this.state.selectedRequest.requestDt;
        let formatDate = moment(requestDate).format('MMDDYYYY');
        return request.requestNbr + request.consultantFirstNm[0].toString() + request.consultantLastNm + formatDate + '.pdf';
    }
}
