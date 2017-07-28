import { Component } from '@angular/core';
import { IdentityService } from './shared';

@Component({
    selector: 'index',
    templateUrl: './index.component.html'
})
export class IndexComponent {

    isCollapsed: boolean = true;

    constructor(private identity: IdentityService) { }

    toggleCollapsed(): void {
        this.isCollapsed = !this.isCollapsed;
    }

    logOut(): void {
        this.identity.logOut().subscribe();
    }
}
