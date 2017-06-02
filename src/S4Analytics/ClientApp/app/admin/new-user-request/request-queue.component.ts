import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import {
    NewUserRequestStateService, NewUserRequestService, QueueColumn, QueueFilter, NewUserRequest, NewUserRequestStatus
} from './shared';



@Component({
    templateUrl: './request-queue.component.html'
})


export class RequestQueueComponent {

    queueFilter = QueueFilter;
    queueColumn = QueueColumn;
    closeResult: string;

    get filteredRequests(): NewUserRequest[] {
        let queueFilter = this.state.queueFilter;
        return _.filter(this.state.newUserRequests,
            (nur: NewUserRequest) => {
                switch (queueFilter) {
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

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.newUserRequestService.getNewUserRequests().subscribe(result => this.state.newUserRequests = result);
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
        this.state.queueFilter = filter;
     }

    openActionModal(content: any, request: NewUserRequest) {
        this.state.selectedRequest = request;
        this.state.currentActionForm = this.modalService.open(content, { backdrop: 'static', keyboard: false });
    }

    hideProcessRequestButton(request: NewUserRequest) {
        return request.requestStatus === NewUserRequestStatus.Completed || request.requestStatus === NewUserRequestStatus.Rejected;
    }

    totalRequestCount(): number {
        return (this.state.newUserRequests === undefined) ? 0 : this.state.newUserRequests.length;
    }

    requestShowingCount(): number {
        return (this.filteredRequests === undefined) ?  0 : this.filteredRequests.length;
    }

}
