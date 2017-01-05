import { Component } from '@angular/core';
import { NewUserRequestStateService, NewAgencyResults } from './shared';

@Component({
    selector: 'new-agency-component',
    template: require('./new-agency.component.html')
})

export class NewAgencyComponent {

    constructor(private state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new NewAgencyResults();
    }
}