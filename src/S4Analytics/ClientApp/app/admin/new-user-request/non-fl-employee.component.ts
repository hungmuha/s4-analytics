import { Component } from '@angular/core';
import { NewUserRequestStateService, NewNonFlAgencyResults } from './shared';

@Component({
    selector: 'new-non-fl-component',
    templateUrl: './non-fl-employee.component.html'
})

export class NewNonFlComponent {

    constructor(private state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new NewNonFlAgencyResults();
    }

}