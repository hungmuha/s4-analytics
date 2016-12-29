import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NewUserRequestStateService, NewUserRequestService } from '../shared';

@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.state.newUserRequests = this.newUserRequestService.getNewUserRequests();
    }
}
