import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    userName: string;
    password: string;

    constructor(
        private http: Http,
        private router: Router) {
        this.userName = '';
        this.password = '';
    }

    logIn(): void {
        this.http
            .post('api/login', { userName: this.userName, password: this.password })
            .map(response => response.json())
            .subscribe((data: any) => {
                if (data.success) {
                    this.router.navigate(['']);
                }
            });
    }
}
