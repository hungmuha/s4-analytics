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
    newUserRequestStatus = NewUserRequestStatus;

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
                console.log('ok');
                this.processOKResult();
            }
            else {
                this.processRejectedResult();
            }


            if (this.state.contractViewerWindow != undefined) {
                this.state.contractViewerWindow.close();
                delete this.state.contractViewerWindow;
            }

        });
    }

    sortColumn(columnNum: number): void {

        if (columnNum != undefined && columnNum === this.state.sortColumn) {
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

    private processOKResult(): void {
        console.log(this.newUserRequestStatus[this.state.selectedRequest.requestStatus]);
        console.log(this.newUserRequestStatus.NewUser);

        switch (this.state.selectedRequest.requestStatus) {
            case this.newUserRequestStatus.NewUser:
                console.log('create new user, record completed');

                this.processNewUserCreated();
                break;
            //case this.newUserRequestStatus.NewConsultant:
            //    console.log('create new consultant, record completed');

            //    this.processNewConsulantCreated();
            //    if (this.sendEmail(this.getNewConsultantCredentialsEmail())) {
            //        this.updateStatus(NewUserRequestStatus.Completed);
            //    }
            //    break;
            //case this.newUserRequestStatus.NewContractor:
            //    console.log('create new contractor, update status to NewConsultant, open NewConsultant dialog');

            //    this.processNewContractorCreated();
            //    break;
            //case this.newUserRequestStatus.NewAgency:
            //    console.log('send email to global admin to create new agency, update status to CreateAgency');

            //    this.processRequestNewAgency();
            //    break;
            //case this.newUserRequestStatus.CreateAgency:
            //    console.log('verify new agency created, update status to NewUser');
            //    this.processNewAgencyCreated();
            //    break;
            default:
                console.log("default");
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
    }

    private processNewUserCreated(): void {
        this.createNewUser();
        this.updateStatus(NewUserRequestStatus.Completed);
        this.sendEmail(this.getNewUserCredentialsEmail());
        this.refreshQueue();
    }

    //private processNewConsulantCreated(): void {
    //    this.createNewConsultant();
    //    this.updateStatus(NewUserRequestStatus.Completed);
    //    this.sendEmail(this.getNewConsultantCredentialsEmail());
    //    this.refreshQueue();
    //}

    //private processRequestNewAgency(): void {
    //    this.updateStatus(NewUserRequestStatus.CreateAgency);
    //    this.sendEmail(this.getCreateAgencyEmail());
    //    this.refreshQueue();
    //}

    //private processNewAgencyCreated(): void {
    //    this.updateStatus(NewUserRequestStatus.NewUser);
    //    this.sendEmail(this.getApproveNewUserEmail());
    //    this.refreshQueue();
    //}
    //private processNewContractorCreated(): void {
    //    this.updateStatus(NewUserRequestStatus.NewConsultant);
    //    // open New Consultant dialog
    //}

    private sendEmail(emailContent: string): void {
        console.log(emailContent);

    }

    private getNewUserCredentialsEmail(): string {

        return '';
    }

    //private getNewConsultantCredentialsEmail(): string {

    //    return '';
    //}

    //private getCreateAgencyEmail(): string {

    //    return '';
    //}

    //private getApproveNewUserEmail(): string {

    //    return '';
    //}

    private createNewUser(): void {
        // create a new user in database

        this.updateCreatedDt(Date.now());
    }

    //private createNewConsultant(): void {
    //    // create a new consultant in database

    //    this.updateCreatedDt(Date.now());
    //}

    private updateCreatedDt(date: number) {
        this.state.selectedRequest.userCreatedDt = new Date(date);
    }

    private updateStatus(status: NewUserRequestStatus)
    {
        this.state.selectedRequest.requestStatus = status;
    }

    private refreshQueue(): void {

    }
}
