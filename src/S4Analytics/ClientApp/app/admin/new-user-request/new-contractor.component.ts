import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-contractor-component',
    templateUrl: './new-contractor.component.html'
})

export class NewContractorComponent {
    errorType: string = 'requestor';
    errorMsg: string = this.state.selectedRequest.warnRequestorEmailCd;

    constructor(public state: NewUserRequestStateService) {

    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=400,height=200');
    }
}
