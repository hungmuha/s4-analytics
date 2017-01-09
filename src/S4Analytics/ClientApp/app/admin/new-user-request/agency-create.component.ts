import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'agency-create-component',
    template: require('./agency-create.component.html')
})

export class AgencyCreateComponent {

    constructor(public state: NewUserRequestStateService) { }
}