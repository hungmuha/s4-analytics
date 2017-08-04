import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import {
   NewUserRequestStateService, NewUserRequestService, QueueColumn, QueueFilter, NewUserRequest, NewUserRequestStatus
} from './shared';
import { IdentityService } from '././../../shared';

@Component({
    selector: 'request-queue',
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
        public state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal,
        private identityService: IdentityService
       ) {
    }

    ngOnInit() {
        this.newUserRequestService.getNewUserRequests(this.identityService.currentUser.userName).subscribe(result => this.state.newUserRequests = result);
    }

    sortColumn(columnName: string): void {
        this.state.sortAsc = !this.state.sortAsc;
        this.state.sortColumnName = columnName;
    }

    filterQueueBy(filter: QueueFilter) {
        this.state.queueFilter = filter;
     }

    openActionModal(content: any, request: NewUserRequest) {
        this.state.selectedRequest = request;
        this.state.currentActionForm = this.modalService.open(content, {backdrop: 'static', keyboard: false });
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

    caretClass(columnName: string): string {
        if (this.state.sortColumnName === columnName && this.state.sortAsc) {
            return 'fa fa-caret-up';
        }
        else if ((this.state.sortColumnName === columnName && !this.state.sortAsc)) {
            return 'fa fa-caret-down';
        }

        return '';
     }


    hideCaretDown(columnName: string) {
        return !(this.state.sortColumnName === columnName && !this.state.sortAsc);
    }

    hideCaretUp(columnName: string) {
        return !(this.state.sortColumnName === columnName && this.state.sortAsc);
    }
}
