import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
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

    toggleCollapsed(): void {
        this.isCollapsed = !this.isCollapsed;
    }

    logIn(): void {
        this.router.navigate(['login']);
    }

    logOut(): void {
        this.identity.logOut().subscribe();
    }
}
