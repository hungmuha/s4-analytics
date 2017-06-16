import { Component } from '@angular/core';
import { IdentityService } from './shared';

@Component({
    selector: 'index',
    templateUrl: './index.component.html'
})
export class IndexComponent {

    constructor(private identity: IdentityService) { }

    logOut(): void {
        this.identity.logOut().subscribe();
    }
}
