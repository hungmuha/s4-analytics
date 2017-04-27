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
        // construct the path and query string for the image handler  \uploads

        this.state.contractViewerWindow = window.open('', '_blank', 'width=400,height=200');
    }
}
