import { Component } from '@angular/core';
import { IdentityService } from './shared';
import { AppStateService } from './app-state.service';

@Component({
    selector: 'index',
    templateUrl: './index.component.html'
})
export class IndexComponent {

    constructor(
        private identity: IdentityService,
        private state: AppStateService) { }

    logOut(): void {
        this.identity.logOut().subscribe();
    }

    toggleMenu(): void {
        this.state.isMenuVisible = !this.state.isMenuVisible;
    }
}
