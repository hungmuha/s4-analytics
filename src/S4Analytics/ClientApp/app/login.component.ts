import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { IdentityService } from './shared';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    userName: string;
    password: string;

    constructor(
        private router: Router,
        private identity: IdentityService) {
        this.userName = '';
        this.password = '';
    }

    logIn(): void {
        this.identity
            .logIn(this.userName, this.password)
            .subscribe(status => {
                if (status.success) {
                    this.router.navigate(['']);
                }
            });
    }
}
