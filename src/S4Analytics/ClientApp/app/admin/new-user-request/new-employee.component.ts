import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-employee-component',
    templateUrl: './new-employee.component.html'
})

export class NewEmployeeComponent {

    constructor(public state: NewUserRequestStateService) {
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }




}