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
        switch (this.state.selectedRequest.requestStatus) {
            case NewUserRequestStatus.NewUser:
                console.log('create new user, record completed');

                this.processNewUserCreated();
                break;
            case NewUserRequestStatus.NewConsultant:
                console.log('create new consultant, record completed');

                this.processNewConsultantCreated();
                if (this.sendEmail(this.getNewConsultantCredentialsEmail())) {
                    this.updateStatus(NewUserRequestStatus.Completed);
                }
                break;
            case NewUserRequestStatus.NewContractor:
                console.log('create new contractor, update status to NewConsultant, open NewConsultant dialog');

                this.processNewContractorCreated();
                break;
            case NewUserRequestStatus.NewAgency:
                console.log('send email to global admin to create new agency, update status to CreateAgency');

                this.processRequestNewAgency();
                break;
            case NewUserRequestStatus.CreateAgency:
                console.log('verify new agency created, update status to NewUser');
                this.processNewAgencyCreated();
                break;
            default:
                console.log('default');
                break;
        }
    }

    private processRejectedResult(): void {

        switch (this.state.selectedRequest.requestStatus) {
            case NewUserRequestStatus.NewUser:
                console.log('email requestor why rejected');
                break;
            case NewUserRequestStatus.NewConsultant:
                console.log('email requestor why rejected');
                break;
            case NewUserRequestStatus.NewContractor:
                console.log('email requestor why rejected');
                break;
            case NewUserRequestStatus.NewAgency:
                console.log('email requestor why rejected');
                break;
            case NewUserRequestStatus.CreateAgency:
                console.log('email requestor why rejected');
                break;
            default:
                console.log('default');
                break;
        }

        this.updateStatus(NewUserRequestStatus.Rejected);

    }

    private processNewUserCreated(): void {
        this.createNewUser();
        this.updateStatus(NewUserRequestStatus.Completed);
        this.sendEmail(this.getNewUserCredentialsEmail());
    }

    private processNewConsultantCreated(): void {
        this.createNewConsultant();
        this.updateStatus(NewUserRequestStatus.Completed);
        this.sendEmail(this.getNewConsultantCredentialsEmail());
    }

    private processRequestNewAgency(): void {
        this.updateStatus(NewUserRequestStatus.CreateAgency);
        this.sendEmail(this.getCreateAgencyEmail());
    }

    private processNewAgencyCreated(): void {
        this.updateStatus(NewUserRequestStatus.NewUser);
        this.sendEmail(this.getApproveNewUserEmail());
    }
    private processNewContractorCreated(): void {
        this.updateStatus(NewUserRequestStatus.NewConsultant);
        // open New Consultant dialog
    }

    private sendEmail(emailContent: string): void {
        console.log(emailContent);

    }

    private getNewUserCredentialsEmail(): string {

        return '';
    }

    private getNewConsultantCredentialsEmail(): string {

        return '';
    }

    private getCreateAgencyEmail(): string {

        return '';
    }

    private getApproveNewUserEmail(): string {

        return '';
    }

    private createNewUser(): void {
        // create a new user in database

        this.updateCreatedDt(Date.now());
    }

    private createNewConsultant(): void {
        // create a new consultant in database

        this.updateCreatedDt(Date.now());
    }

    private updateCreatedDt(date: number) {
        this.state.selectedRequest.userCreatedDt = new Date(date);
    }

    private updateStatus(status: NewUserRequestStatus) {
        this.state.selectedRequest.requestStatus = status;
    }




    get hideRequestorWarning() {
        return this.state.selectedRequest.warnRequestorEmailCd === 'N';
    }

    get hideConsultantWarning() {
        return this.state.selectedRequest.warnConsultantEmailCd === 'N';
    }



}
