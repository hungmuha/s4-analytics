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
            console.log('approved');
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=400,height=200');
    }
}
