import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { IdentityService } from './identity.service';
import { Observable } from 'rxjs/Observable';
import * as _ from 'lodash';

@Injectable()
export class AuthGuard implements CanActivate, CanActivateChild {
    protected roles: string[];

    constructor(
        private identityService: IdentityService,
        private router: Router) { }

    canActivate(_: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean {
        if (this.identityService.checkedForServerSession || this.identityService.isAuthenticated) {
            return this.checkUserAuth(state.url);
        }
        else {
            return this.identityService
                .checkForServerSession()
                .map(() => this.checkUserAuth(state.url));
        }
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean {
        return this.canActivate(route, state);
    }

    private checkUserAuth(url: string): boolean {

        // authentication check
        if (!this.identityService.isAuthenticated) {
            // go to login
            this.identityService.redirectUrl = url;
            this.router.navigate(['login']);
            return false;
        }

        // authorization (role) check
        let roleMatch =
            this.roles === undefined ||
            _.intersection(this.roles, this.identityService.currentUser.roles).length > 0;
        if (!roleMatch) {
            // go to index
            this.router.navigate(['']);
            return false;
        }

        return true;
    }
}

@Injectable()
export class AnyAdminGuard extends AuthGuard {
    constructor(identityService: IdentityService, router: Router) {
        super(identityService, router);
        this.roles = ['global admin', 'agency admin'];
    }
}

@Injectable()
export class GlobalAdminGuard extends AuthGuard {
    constructor(identityService: IdentityService, router: Router) {
        super(identityService, router);
        this.roles = ['global admin'];
    }
}
