import { Component } from '@angular/core';
import { IdentityService } from './shared';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    userName = '';
    password = '';
    failedAttempt = false;

    constructor(private identity: IdentityService) { }

    logIn(): void {
        this.identity
            .logIn(this.userName, this.password)
            .subscribe(success => {
                this.failedAttempt = !success;
                setTimeout(() => this.failedAttempt = false, 3000);
            });
    }
}
