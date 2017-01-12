import { NewUserRequestStateService, RequestActionResults } from './shared';

export class RequestActionComponent {
    constructor(public state: NewUserRequestStateService) {
        this.state.currentRequestActionResults = new RequestActionResults();
    }

    private get hideRequestorWarning() {
        return this.state.selectedRequest.warnRequestorEmailCd == 'Y';
    }
}