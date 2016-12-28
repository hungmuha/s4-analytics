import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'newuserrequestprocess',
    template: require('./new-user-request-detail.component.html')
})
export class NewUserRequestDetailComponent {
    constructor(
        private router: Router
    ) { }
}