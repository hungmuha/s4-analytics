import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent  {
    errorType: string = 'consultant';
    errorMsg: string = this.state.selectedRequest.warnConsultantEmailCd;


    constructor(public state: NewUserRequestStateService) {

    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=200,height=100');
    }
}

