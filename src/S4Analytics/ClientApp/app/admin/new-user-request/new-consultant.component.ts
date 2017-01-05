import { Component } from '@angular/core';
import { NewUserRequestStateService, RequestActionResults } from './shared';

@Component({
    selector: 'new-consultant-component',
    template: require('./new-consultant.component.html')
})

export class NewConsultantComponent {

    constructor(private state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new RequestActionResults();
    }
}

