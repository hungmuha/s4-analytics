import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'request-warning-component',
    templateUrl: './request-warning.component.html'
})

export class RequestWarningComponent {
    constructor(public state: NewUserRequestStateService) {
    }
}
