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
            console.log('approved');
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            console.log('disproved');
            (<NewAgencyResults>(this.state.currentRequestActionResults)).lea = undefined;
            (<NewAgencyResults>(this.state.currentRequestActionResults)).accessBefore70Days = false;
        }
    }


}