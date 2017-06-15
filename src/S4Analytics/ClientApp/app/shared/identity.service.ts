import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

export interface AuthenticationStatus {
    success: boolean;
}

@Injectable()
export class IdentityService {
    constructor(private http: Http) { }

    logIn(userName: string, password: string): Observable<AuthenticationStatus> {
        return this.http
            .post('api/identity/login', { userName: userName, password: password })
            .map(response => response.json() as AuthenticationStatus);
    }

    logOut(): Observable<AuthenticationStatus> {
        return this.http
            .post('api/identity/logout', {})
            .map(response => response.json() as AuthenticationStatus);
    }
}
