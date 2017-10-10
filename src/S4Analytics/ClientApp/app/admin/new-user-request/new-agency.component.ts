import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-agency-component',
    templateUrl: './new-agency.component.html'
})

export class NewAgencyComponent {

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
}