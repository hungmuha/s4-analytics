import { Component } from '@angular/core';
import { NewUserRequestStateService, NewConsultantActionResults } from './shared';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {

    constructor(public state: NewUserRequestStateService) {

    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            let newConsultantResults = this.state.currentRequestActionResults as NewConsultantActionResults;
            newConsultantResults.accessBefore70Days = false;
        }
    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=200,height=100');
    }
}

