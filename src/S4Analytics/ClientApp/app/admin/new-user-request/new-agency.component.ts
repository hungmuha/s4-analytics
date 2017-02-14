import { Component } from '@angular/core';
import { NewUserRequestStateService, NewAgencyResults } from './shared';

@Component({
    selector: 'new-agency-component',
    templateUrl: './new-agency.component.html'
})

export class NewAgencyComponent  {

    newAgencyResults = this.state.currentRequestActionResults as NewAgencyResults;

    disableRejctRb() {
        return this.newAgencyResults.lea === undefined;
    }

    constructor(public state: NewUserRequestStateService) {

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