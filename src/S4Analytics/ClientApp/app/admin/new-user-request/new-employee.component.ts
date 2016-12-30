import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-employee-component',
    template: require('./new-employee.component.html')
})

export class NewEmployeeComponent {
    approveReject: string[] = [
        'Accept Employee',
        'Reject Employee'
    ];

    constructor(private state: NewUserRequestStateService) { }
}