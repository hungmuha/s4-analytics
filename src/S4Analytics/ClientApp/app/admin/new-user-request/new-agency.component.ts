import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-agency-component',
    templateUrl: './new-agency.component.html'
})

export class NewAgencyComponent {

    constructor(public state: NewUserRequestStateService) {
    }

    disableRejectRb() {
        return this.state.currentRequestActionResults.lea === undefined;
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
        else {
            this.state.currentRequestActionResults.lea = undefined;
            this.state.currentRequestActionResults.accessBefore70Days = false;
        }
    }

}