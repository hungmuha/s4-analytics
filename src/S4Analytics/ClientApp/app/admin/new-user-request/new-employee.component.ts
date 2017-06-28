import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-employee-component',
    templateUrl: './new-employee.component.html'
})

export class NewEmployeeComponent {

    constructor(public state: NewUserRequestStateService) {
    }

    ngOnInit() {

       this.state.requestorWarningMessages = [];

        if (this.state.selectedRequest.warnRequestorEmailCd) {
            this.state.requestorWarningMessages.push('Requestor/Agency email domain mismatch');
        }

        if (this.state.selectedRequest.warnDuplicateEmailCd) {
            this.state.requestorWarningMessages.push('An account with email already exists');
        }
    }

    hideRequestorWarning(): boolean {
        if (!this.state.selectedRequest.warnRequestorEmailCd && !this.state.selectedRequest.warnDuplicateEmailCd) {
            return true;
        }

        return false;
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }
}