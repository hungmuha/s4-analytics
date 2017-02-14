import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NewUserRequestStateService, RequestActionResults, NewUserRequestStatus } from './shared';


@Component({
    selector: 'request-action-component',
    templateUrl: './request-action.component.html'
})


export class RequestActionComponent  {

    newUserRequestStatus = NewUserRequestStatus;

    constructor(public state: NewUserRequestStateService, public modalService: NgbModal) {
        if (this.state === undefined) { return; }
        this.state.currentRequestActionResults = new RequestActionResults();
    }

    newUserRequestMatch(nur: number) {
        return this.state.selectedRequest.requestStatus === nur;
    }

    submit() {

        if (this.state.currentRequestActionResults.approved) {
            this.processOKResult();
        }
        else {
            this.processRejectedResult();
        }

        this.state.currentActionForm.close();
        this.closeContractViewer();
    }


    cancel() {
        this.state.currentActionForm.close();
        this.closeContractViewer();
    }

    private closeContractViewer() {
        if (this.state.contractViewerWindow != undefined) {
            this.state.contractViewerWindow.close();
            delete this.state.contractViewerWindow;
        }
    }

    private processOKResult(): void {

    }

    private processRejectedResult(): void {

    }



}
