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
        this.state.warningMessages = [];

        if (this.state.selectedRequest.warnRequestorEmailCd) {
            this.state.warningMessages.push('Requestor/Agency email domain mismatch');
        }

        if (this.state.selectedRequest.warnDuplicateEmailCd) {
            this.state.warningMessages.push('An account with email already exists');
        }
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }
}