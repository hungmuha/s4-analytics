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
                console.log('result: OK');
            }
            else {
                console.log('result: CANCELLED');
            }

            let r = this.state.currentRequestActionResults;

            console.log('approved: ' + r.approved);
            if (r.approved) {
                console.log('User: APPROVED');
            }
            else {
                console.log('User: REJECTED');
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
}
