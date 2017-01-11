import { Component } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NewUserRequestStateService, NewUserRequestService, NewUserRequestStatus} from './shared';

@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {
    closeResult: string;
    index: number = 5;
    requestType = 'New Employee Request';
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

}
