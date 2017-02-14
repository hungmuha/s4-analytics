import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
    NewUserRequestStateService, NewUserRequestService
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
    templateUrl: './request-queue.component.html'
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

    filterQueueBy(button: number) {
        this.newUserRequestService.filterNewUserRequestsBy(button).subscribe(result => this.state.newUserRequests = result);
    }

    openActionModal(content: any, index: number) {
        this.state.selectedRequest = this.state.newUserRequests[index];

        console.log('this: ' + this.state.selectedRequest.warnRequestorEmail);

        this.state.currentActionForm = this.modalService.open(content, { backdrop: 'static', keyboard: false });
    }



}
