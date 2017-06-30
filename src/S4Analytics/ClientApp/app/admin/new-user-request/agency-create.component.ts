﻿import { Component } from '@angular/core';
import { NewUserRequestStateService, NewUserRequestService, RequestActionResults } from './shared';

@Component({
    selector: 'agency-create-component',
    templateUrl: './agency-create.component.html'
})

export class AgencyCreateComponent {

    constructor(private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService) {
        this.state.currentRequestActionResults = new RequestActionResults(this.state.selectedRequest.requestNbr);
    }

    ngOnInit() {
        this.newUserRequestService.doesAgencyExist(this.state.selectedRequest.agncyNm)
            .subscribe(
            result => {
                this.state.currentRequestActionResults.agencyCreated = result;
            }
            );
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }


}