import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
    NewUserRequestStateService, NewUserRequestService, NewUserRequestStatus
} from './shared';

export enum QueueColumn {
    ReqNbr,
    ReqDt,
    ReqType,
    Requestor,
    ReqAgncy,
    ReqStatus,
    AcctCreated,
    Comment
}

@Component({
    template: require('./request-queue.component.html')
})


export class RequestQueueComponent {

    closeResult: string;

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.newUserRequestService.getNewUserRequests().subscribe(result => this.state.newUserRequests = result);
    }

    openActionModal(content: any, index: number) {
        this.state.selectedRequest = this.state.newUserRequests[index];
        let actionModal = this.modalService.open(content, { backdrop: 'static', keyboard: false });

        actionModal.result.then((result) => {

            if (result === 'ok') {
                if (this.state.currentRequestActionResults.approved) {
                    this.processOKResult();
                }
                else {
                    this.processRejectedResult();
                }
            }

            // what to do if cancel  --> reset state?
            if (this.state.contractViewerWindow != undefined) {
                this.state.contractViewerWindow.close();
                delete this.state.contractViewerWindow;
            }

        });
    }

    sortColumn(columnNum: number): void {

        if (columnNum === this.state.sortColumn) {
            this.state.sortAsc = !this.state.sortAsc;
        }
        else {
            this.state.sortColumn = columnNum;
        }

        let sortOrder = this.state.sortAsc ? '' : '!';

        switch (columnNum) {
            case 1: this.state.sortField = [sortOrder + 'requestNbr']; break;
            case 2: this.state.sortField = [sortOrder + 'requestDt']; break;
            case 3: this.state.sortField = [sortOrder + 'requestType']; break;
            case 4: this.state.sortField = [sortOrder + 'requestorLastNm']; break;
            case 5: this.state.sortField = [sortOrder + 'agncyNm']; break;
            case 6: this.state.sortField = [sortOrder + 'requestStatus']; break;
            case 7: this.state.sortField = [sortOrder + 'userCreatedDt']; break;
            case 8: this.state.sortField = [sortOrder + 'adminComment']; break;
            default: this.state.sortField = [sortOrder + 'requestNbr']; break;
        }
    }

    get okButtonDisabled() {
        if (this.state.currentRequestActionResults.rejectionReason != undefined) {
            console.log(this.state.currentRequestActionResults.rejectionReason.trim.length === 0);
        }

        return this.state.currentRequestActionResults.approved == undefined ||
            (!this.state.currentRequestActionResults.approved &&
                (this.state.currentRequestActionResults.rejectionReason == undefined ||
                    this.state.currentRequestActionResults.rejectionReason.trim().length === 0));
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

        this.updateStatus(NewUserRequestStatus.Rejected);

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
        console.log('rejected');

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
        console.log(status);
        this.state.selectedRequest.requestStatus = status;
        console.log(this.state.selectedRequest.requestStatus);
    }
}
