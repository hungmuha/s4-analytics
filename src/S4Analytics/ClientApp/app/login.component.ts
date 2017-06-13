import { Component } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'login',
    templateUrl: './login.component.html'
})
export class LoginComponent {
    userName: string;
    password: string;

    constructor(private http: Http) { }

    logIn(): void {
        this.http.get(`api/login/${this.userName}`).subscribe();
    }
}
