import { Component } from '@angular/core';
import { NewUserRequestStateService, NewAgencyActionResults } from './shared';

@Component({
    selector: 'new-agency-component',
    templateUrl: './new-agency.component.html'
})

export class NewAgencyComponent {

    newAgencyResults = this.state.currentRequestActionResults as NewAgencyActionResults;

    constructor(public state: NewUserRequestStateService) {
    }

    disableRejectRb() {
        return this.newAgencyResults.lea === undefined;
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            this.newAgencyResults.lea = undefined;
            this.newAgencyResults.accessBefore70Days = false;
        }
    }


}