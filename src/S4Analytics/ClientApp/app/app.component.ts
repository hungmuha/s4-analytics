import { Component } from '@angular/core';
import { IdentityService } from './shared';

@Component({
    selector: 'app',
    templateUrl: './app.component.html'
})
export class AppComponent {
    isCollapsed: boolean = true;

    constructor(private identity: IdentityService) { }

    toggleCollapsed(): void {
        this.isCollapsed = !this.isCollapsed;
    }

    logOut(): void {
        this.identity.logOut().subscribe();
    }
}
