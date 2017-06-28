import { Component } from '@angular/core';
import { NewUserRequestStateService, NewUserRequestService, RequestActionResults } from './shared';

@Component({
    selector: 'agency-create-component',
    templateUrl: './agency-create.component.html'
})

export class AgencyCreateComponent {

    agencyExists: boolean = false;

    constructor(private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService) {
        this.state.currentRequestActionResults = new RequestActionResults(this.state.selectedRequest.requestNbr);
    }

    ngOnInit() {
        this.doesAgencyExist();
        
    }

    approved(approved: boolean) {

        if (approved) {
            this.state.currentRequestActionResults.rejectionReason = '';
        }
    }

    doesAgencyExist(): void {
        this.newUserRequestService.verifyAgency(this.state.selectedRequest.agncyNm)
            .subscribe(
            result => { this.agencyExists = result != 0; }
        );
    }

    //checkAgencyExists() {
    //    this.newUserRequestService.verifyAgency(this.state.selectedRequest.agncyNm)
    //        .subscribe(
    //       result => {
    //           console.log('back');
    //           let agencyId = result;
    //           if (agencyId === 0) {
    //               alert("Not found");
    //           }
    //           else {
    //               alert("Found");
    //           }
    //        });
    //}

}