import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import { IdentityService, S4IdentityUser } from '../../shared';
import {
    NewUserRequestStateService, NewUserRequestService, QueueColumn,
    QueueFilter, NewUserRequest, NewUserRequestStatus
} from './shared';

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
                        return nur.requestStatus !== NewUserRequestStatus.Completed
                            && nur.requestStatus !== NewUserRequestStatus.Rejected;
                    default:
                        return true;
                }
            });
    }

    constructor(
        public state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal,
        private identityService: IdentityService) { }

    ngOnInit() {
        this.newUserRequestService
            .getNewUserRequests()
            .subscribe(result => this.state.newUserRequests = result);
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
        this.state.isSelectedRequestLocked = this.isRequestLocked(request);
        this.state.currentActionForm.modalRef = this.modalService.open(content, {backdrop: 'static', keyboard: false });
    }

    isRequestLocked(request: NewUserRequest) {
        let currentUser = this.identityService.currentUser as S4IdentityUser;

        return request.requestStatus === NewUserRequestStatus.Completed
            || request.requestStatus === NewUserRequestStatus.Rejected


            || (currentUser.roles.indexOf('global admin') > -1 // global admin should only act on create agency tasks
                && (request.requestStatus !== NewUserRequestStatus.CreateAgency
            && !(request.requestStatus === NewUserRequestStatus.NewUser && !request.hasAdmin)) )

            || ((currentUser.roles.indexOf('hsmv admin') > -1 // hsmv admin should not act on create agency tasks
                && request.requestStatus === NewUserRequestStatus.CreateAgency))

            || ((currentUser.roles.indexOf('fdot admin') > -1 // FDOT admin should not act on create agency tasks
                && request.requestStatus === NewUserRequestStatus.CreateAgency));
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
