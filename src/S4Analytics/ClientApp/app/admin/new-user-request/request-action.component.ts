import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { IdentityService, S4IdentityUser } from '../../shared';
import {
    NewUserRequestStateService, NewUserRequestService,
    NewUserRequest, NewUserRequestStatus,
    RequestActionResults
} from './shared';


@Component({
    selector: 'request-action-component',
    templateUrl: './request-action.component.html'
})


export class RequestActionComponent  {

    newUserRequestStatus = NewUserRequestStatus;
    closeDialog = true;
    agencyExists: boolean = false;

    constructor(public state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        public modalService: NgbModal,
        private identityService: IdentityService
        ) {
        this.state.currentRequestActionResults = new RequestActionResults(this.state.selectedRequest.requestNbr);
    }

    newUserRequestMatch(nur: number) {

        return this.state.selectedRequest.requestStatus === nur;
    }

    hideReasonTextArea() {
        return this.state.currentRequestActionResults.approved === undefined || this.state.currentRequestActionResults.approved;
    }

    hideReportAccessCb() {
        return (!this.state.currentRequestActionResults.approved &&
           ( (this.state.selectedRequest.requestStatus === NewUserRequestStatus.NewConsultant) ||
            (this.state.selectedRequest.requestStatus === NewUserRequestStatus.NewVendor) ||
                (this.state.selectedRequest.requestStatus === NewUserRequestStatus.NewAgency)))
            ||
            ((this.state.selectedRequest.requestStatus !== NewUserRequestStatus.NewConsultant) &&
                (this.state.selectedRequest.requestStatus !== NewUserRequestStatus.NewVendor) &&
                (this.state.selectedRequest.requestStatus !== NewUserRequestStatus.NewAgency));
    }

    hideRejectButton(request: NewUserRequest) {
        let currentUser = this.identityService.currentUser as S4IdentityUser;

        return (this.state.selectedRequest.requestStatus === NewUserRequestStatus.CreateAgency)
            && (currentUser.roles.indexOf('global admin') > -1);

    }

    disableOKButton() {
        // disable if trying to approve the creation of an agency that has not been created, even if rest of form is valid
        return (this.state.currentRequestActionResults.approved && !this.state.currentRequestActionResults.agencyCreated);
    }

    submit() {
        if (this.state.currentRequestActionResults.approved) {
            this.processOKResult();
        }
        else {
            this.processRejectedResult();
        }

        this.closeContractViewer();
    }

    cancel() {
        this.state.currentActionForm.close();
        this.closeContractViewer();
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
        else {
            this.state.currentRequestActionResults.accessBefore70Days = false;
        }
    }

    private closeContractViewer() {
        if (this.state.contractViewerWindow != undefined) {
            this.state.contractViewerWindow.close();
            this.state.contractViewerWindow = undefined;
        }
    }

    private processOKResult(): void {
        this.newUserRequestService.approve(this.state.selectedRequest, this.state.currentRequestActionResults)
            .subscribe(
            result => {
                this.state.selectedRequest = result;
                let index = this.state.newUserRequests.findIndex(newUserReq =>
                    newUserReq.requestNbr === this.state.selectedRequest.requestNbr);

                this.state.newUserRequests[index] = this.state.selectedRequest;
                this.state.currentActionForm.close();
            });
    }

    private processRejectedResult(): void {
        this.newUserRequestService.reject(this.state.currentRequestActionResults, this.state.selectedRequest)
            .subscribe(
            result => {
                this.state.selectedRequest = result;
                let index = this.state.newUserRequests.findIndex(newUserReq => newUserReq.requestNbr === this.state.selectedRequest.requestNbr);
                this.state.newUserRequests[index] = this.state.selectedRequest;
                this.state.currentActionForm.close();
            });
    }



}
