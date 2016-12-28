import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'newuserrequestqueue',
    template: require('./new-user-request-master.component.html')
})
export class NewUserRequestMasterComponent {
    constructor(
        private router: Router
    ) { }

    /* tslint:disable:no-unused-variable */

    private processRequest(): void {
        let processRoute = ['/newuserrequestprocess'];
        this.router.navigate(processRoute);
    }

    /* tslint:enable:no-unused-variable */
}
