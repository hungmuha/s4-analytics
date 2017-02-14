import { Component } from '@angular/core';
import { NewUserRequestStateService, NewAgencyResults } from './shared';

@Component({
    selector: 'new-agency-component',
    templateUrl: './new-agency.component.html'
})

export class NewAgencyComponent  {
    constructor(public state: NewUserRequestStateService) {

    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            let newAgencyResults = this.state.currentRequestActionResults as NewAgencyResults;
            newAgencyResults.lea = undefined;
            newAgencyResults.accessBefore70Days = false;
        }
    }


}