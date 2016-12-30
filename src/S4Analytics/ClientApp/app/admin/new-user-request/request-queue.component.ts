import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NewUserRequestStateService, NewUserRequestService, NewUserRequest } from './shared';

@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {
    closeResult: string;
    index: number = 1;

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.state.newUserRequests = this.newUserRequestService.getNewUserRequests();
    }

}
