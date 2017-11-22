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

export class RequestActionComponent {

    newUserRequestStatus = NewUserRequestStatus;
    closeDialog = true;
    agencyExists: boolean = false;

    constructor(public state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        public modalService: NgbModal,
        private identityService: IdentityService) {
        this.state.currentRequestActionResults = new RequestActionResults(this.state.selectedRequest.requestNbr);
    }

    requestHasStatus(status: NewUserRequestStatus) {
        let initStatus = this.state.selectedRequest.initialRequestStatus;
        let currStatus = this.state.selectedRequest.requestStatus;
        if (currStatus === NewUserRequestStatus.Completed || currStatus === NewUserRequestStatus.Rejected) {
            // if the request has been closed (completed or rejected), use its initial status
            return status === initStatus;
        }
        else {
            // otherwise use its current status
            return status === currStatus;
        }
    }

    hideReasonTextArea() {
        return this.state.currentRequestActionResults.approved === undefined || this.state.currentRequestActionResults.approved;
    }

    hideReportAccessCb() {

        return (!this.state.currentRequestActionResults.approved)
            ||
            (((this.state.selectedRequest.requestStatus === NewUserRequestStatus.NewConsultant) ||
                (this.state.selectedRequest.requestStatus === NewUserRequestStatus.NewVendor)) &&
                !this.state.selectedRequest.accessBefore70Days);
    }

    hideRejectButton(request: NewUserRequest) {
        let currentUser = this.identityService.currentUser as S4IdentityUser;

        return (this.state.selectedRequest.requestStatus === NewUserRequestStatus.CreateAgency)
            && (currentUser.roles.indexOf('global admin') > -1);
    }

    validDate(dateStr: string) {
        return this.state.dateRegex.test(dateStr);
    }

    disableOKButton() {
        return (this.state.currentRequestActionResults.approved && (!this.state.currentActionForm.valid));
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
