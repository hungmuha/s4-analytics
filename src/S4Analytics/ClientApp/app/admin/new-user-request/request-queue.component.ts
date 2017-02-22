import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import {
    NewUserRequestStateService, NewUserRequestService, QueueColumn, NewUserRequest, NewUserRequestStatus
} from './shared';



export enum QueueFilter {
    All,
    Pending,
    Completed,
    Rejected
}

@Component({
    templateUrl: './request-queue.component.html'
})


export class RequestQueueComponent {

    queueFilter = QueueFilter;
    queueColumn = QueueColumn;
    closeResult: string;

    filteredRequests: NewUserRequest[] = this.state.newUserRequests;

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.newUserRequestService.getNewUserRequests().subscribe(result => this.state.newUserRequests = this.filteredRequests = result);
    }

    sortColumn(columnNum: QueueColumn): void {

        if (columnNum === this.state.sortColumn) {
            this.state.sortAsc = !this.state.sortAsc;
        }
        else {
            this.state.sortColumn = columnNum;
        }

        let sortOrder = this.state.sortAsc ? '' : '!';

        switch (columnNum) {
            case QueueColumn.ReqNbr: this.state.sortField = [sortOrder + 'requestNbr']; break;
            case QueueColumn.ReqDt: this.state.sortField = [sortOrder + 'requestDt']; break;
            case QueueColumn.ReqType: this.state.sortField = [sortOrder + 'requestType']; break;
            case QueueColumn.Requestor: this.state.sortField = [sortOrder + 'requestorLastNm']; break;
            case QueueColumn.ReqAgncy: this.state.sortField = [sortOrder + 'agncyNm']; break;
            case QueueColumn.ReqStatus: this.state.sortField = [sortOrder + 'requestStatus']; break;
            case QueueColumn.AcctCreated: this.state.sortField = [sortOrder + 'userCreatedDt']; break;
            case QueueColumn.Comment: this.state.sortField = [sortOrder + 'adminComment']; break;
            default: this.state.sortField = [sortOrder + 'requestNbr']; break;
        }
    }

    filterQueueBy(filter: QueueFilter) {
        this.filteredRequests = _.filter(this.state.newUserRequests,
            function (nur) {
                switch (filter.valueOf()) {
                    case QueueFilter.Completed:
                        return nur.requestStatus === NewUserRequestStatus.Completed;
                    case QueueFilter.Rejected:
                        return nur.requestStatus === NewUserRequestStatus.Rejected;
                    case QueueFilter.Pending:
                        return nur.requestStatus !== NewUserRequestStatus.Completed && nur.requestStatus !== NewUserRequestStatus.Rejected;
                    default:
                        return true;

                }
            });
    }

    filterRequests() { return this.filteredRequests; }

    openActionModal(content: any, index: number) {
        this.state.selectedRequest = this.state.newUserRequests[index];
        this.state.currentActionForm = this.modalService.open(content, { backdrop: 'static', keyboard: false });
    }

    displayAgencyNm(nur: NewUserRequest): string {
        return nur.newAgncyNm === undefined ? nur.agncyNm : nur.newAgncyNm;
    }

    hideProcessRequestButton(index: number) {
        let request = this.state.newUserRequests[index];
        return request.requestStatus === NewUserRequestStatus.Completed || request.requestStatus === NewUserRequestStatus.Rejected;
    }

}
