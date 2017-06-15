import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { IdentityService } from './shared';

@Component({
    selector: 'index',
    templateUrl: './index.component.html'
})
export class IndexComponent {

    constructor(
        private router: Router,
        private identity: IdentityService) { }

    logOut(): void {
        this.identity
            .logOut()
            .subscribe(status => {
                if (status.success) {
                    this.router.navigate(['', 'login']);
                }
            });
    }
}
