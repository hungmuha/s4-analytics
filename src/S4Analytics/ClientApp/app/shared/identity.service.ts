import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { S4IdentityUser } from './s4-identity-user';

@Injectable()
export class IdentityService {
    checkedForServerSession = false;
    redirectUrl?: string; // set by AuthGuardService before navigating to login
    currentUser?: S4IdentityUser;

    get isAuthenticated(): boolean {
        return this.currentUser !== undefined;
    }

    constructor(private http: Http, private router: Router) { }

    logIn(userName: string, password: string): Observable<boolean> {
        return this.http
            .post('api/identity/login', { userName: userName, password: password })
            .map(response => response.json() as S4IdentityUser)
            .map(user => {
                this.currentUser = user;
                let url = this.redirectUrl || '/';
                this.redirectUrl = undefined;
                this.router.navigateByUrl(url);
                return true;
            })
            .catch(() => { // in case of 401 Unauthorized
                this.currentUser = undefined;
                return Observable.of(false);
            });
    }

    logInWithToken(token: string): Observable<boolean> {
        let url = `api/identity/login/${token}`;
        return this.http
            .post(url, {})
            .map(response => response.json() as S4IdentityUser)
            .map(user => {
                this.currentUser = user;
                return true;
            })
            .catch(() => { // in case of 401 Unauthorized
                this.currentUser = undefined;
                return Observable.of(false);
            });
    }

    logOut(): Observable<any> {
        return this.http
            .post('api/identity/logout', {})
            .do(() => {
                this.currentUser = undefined;
                this.router.navigate(['login']);
            });
    }

    checkForServerSession(): Observable<boolean> {
        // It is possible that a user logs in, then browses away from
        // the site or reloads the page. In either case Angular
        // will lose state. This method polls server for the current user.
        // AuthGuardService calls this method just once.
        return this.http
            .get('api/identity/current-user')
            .map(response => {
                let user = response.json() as S4IdentityUser;
                this.checkedForServerSession = true;
                this.currentUser = user;
                return true;
            })
            .catch(() => { // in case of 401 Unauthorized
                this.checkedForServerSession = true;
                this.currentUser = undefined;
                return Observable.of(false);
            });
    }
}
