import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    userName: string;
    password: string;
    success: string;
    currentUser: string;

    constructor(private http: Http) {
        this.userName = "";
        this.password = "";
        this.success = "";
        this.currentUser = "";
    }

    logIn(): void {
        this.http
            .post('api/login', { userName: this.userName, password: this.password })
            .map(response => response.json())
            .subscribe((data: any) => {
                this.success = data.success ? "success" : "failure";
                this.currentUser = "";
                if (this.success) {
                    this.getCurrentUser();
                }
            });
    }

    getCurrentUser(): void {
        this.http
            .get('api/current-user')
            .map(response => response.json())
            .subscribe((data: any) => {
                this.currentUser = data.userName;
            });
    }
}
