import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { IdentityService } from './identity.service';

@Injectable()
export class Html5ConduitResolve implements Resolve<any> {
    constructor(private identityService: IdentityService) { }

    resolve(route: ActivatedRouteSnapshot): Observable<any> {
        let token = route.params['token'] as string;
        return this.identityService.logInWithToken(token);
    }
}
