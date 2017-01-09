import { Component } from '@angular/core';
import { NewUserRequestStateService, RequestActionResults } from './shared';

@Component({
    selector: 'new-contractor-component',
    template: require('./new-contractor.component.html')
})

export class NewContractorComponent {

    constructor(private state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new RequestActionResults();
    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=200,height=100');
    }
}
