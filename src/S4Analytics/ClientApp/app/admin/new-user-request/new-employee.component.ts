import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';

@Component({
    selector: 'new-employee-component',
    templateUrl: './new-employee.component.html'
})

export class NewEmployeeComponent  {


    errorType: string = 'requestor';
    errorMsg: string = this.state.selectedRequest.warnRequestorEmailCd;

    constructor(public state: NewUserRequestStateService) {
    }






}