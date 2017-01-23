import { Component } from '@angular/core';
import { NewUserRequestStateService, RequestActionResults, NewUserRequestStatus } from './shared';


@Component({
    selector: 'request-action-component',
    template: require('./request-action.component.html')
})


export class RequestActionComponent {

    newUserRequestStatus = NewUserRequestStatus;

    constructor(public state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new RequestActionResults();
    }

    newUserRequestMatch(nur: number) {
        return this.state.selectedRequest.requestStatus === nur;
    }

    get hideRequestorWarning() {
        return this.state.selectedRequest.warnRequestorEmailCd === 'N';
    }

    get hideConsultantWarning() {
        return this.state.selectedRequest.warnConsultantEmailCd === 'N';
    }
}
