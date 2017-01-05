import { Component } from '@angular/core';
import { NewUserRequestStateService, RequestActionResults } from './shared';

@Component({
    selector: 'new-employee-component',
    template: require('./new-employee.component.html')
})

export class NewEmployeeComponent {

    constructor(private state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new RequestActionResults();
    }
}