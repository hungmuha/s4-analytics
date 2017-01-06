import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NewUserRequestStateService, NewUserRequestService } from './shared';

@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {
    closeResult: string;
    index: number = 4;
    requestType = 'New Employee Request';

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.state.newUserRequests = this.newUserRequestService.getNewUserRequests();
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

        });
    }

}
