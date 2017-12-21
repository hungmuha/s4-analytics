import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import * as _ from 'lodash';
import { IdentityService, AppStateService, Options } from './shared';

@Component({
    templateUrl: './index.component.html'
})
export class IndexComponent implements OnInit, OnDestroy {
    isCollapsed: boolean = true;

    private routeSub: Subscription;

    constructor(
        private state: AppStateService,
        private route: ActivatedRoute,
        private router: Router,
        private identity: IdentityService) { }

    ngOnInit() {
        this.routeSub = this.route.data
            .subscribe((data: { options: Options }) => {
                this.state.options = data.options;
            });
    }

    ngOnDestroy() {
        this.routeSub.unsubscribe();
    }

    get showResponsiveCues(): boolean {
        return this.state.options ? this.state.options.isDevelopment : false;
    }

    get currentMode(): string {
        if (this.router.url.startsWith('/admin')) {
            return 'Administration';
        }
        else if (this.router.url.startsWith('/reporting')) {
            return 'Reporting';
        }
        return '';
    }

    toggleCollapsed(): void {
        this.isCollapsed = !this.isCollapsed;
    }

    logIn(): void {
        this.router.navigate(['login']);
    }

    logOut(): void {
        this.identity.logOut().subscribe();
    }

    get isAdmin(): boolean {
        let adminRoles = ['global admin', 'user manager', 'hsmv admin', 'fdot admin'];
        return this.identity.currentUser
            ? _.intersection(adminRoles, this.identity.currentUser.roles).length > 0
            : false;
    }
}
