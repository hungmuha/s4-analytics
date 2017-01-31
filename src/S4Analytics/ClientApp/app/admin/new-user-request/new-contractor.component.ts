import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';
import { RequestActionComponent } from './request-action.component';

@Component({
    selector: 'new-contractor-component',
    templateUrl: './new-contractor.component.html'
})

export class NewContractorComponent extends RequestActionComponent {

    constructor(public state: NewUserRequestStateService) {
        super(state);
    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=400,height=200');
    }
}
