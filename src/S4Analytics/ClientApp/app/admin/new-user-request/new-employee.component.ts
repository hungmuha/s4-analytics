import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';
import { RequestActionComponent } from './request-action.component';

@Component({
    selector: 'new-employee-component',
    template: require('./new-employee.component.html')
})

export class NewEmployeeComponent extends RequestActionComponent {

    constructor(public state: NewUserRequestStateService) {
        super(state);
    }
}