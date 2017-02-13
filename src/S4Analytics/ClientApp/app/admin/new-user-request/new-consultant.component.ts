import { Component } from '@angular/core';
import { NewUserRequestStateService, NewAgencyResults } from './shared';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {
    errorType: string = 'consultant';
    errorMsg: string = this.state.selectedRequest.warnConsultantEmailCd;


    constructor(public state: NewUserRequestStateService) {

    }

    approved(approved: boolean) {

        if (approved) {
            console.log('approved');
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            console.log('disproved');
            (<NewAgencyResults>(this.state.currentRequestActionResults)).lea = undefined;
            (<NewAgencyResults>(this.state.currentRequestActionResults)).accessBefore70Days = false;
        }
    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=200,height=100');
    }
}

