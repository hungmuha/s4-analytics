import { Component, Input } from '@angular/core';

@Component({
    selector: 'warning-messages',
    template: `<div style="color:red" *ngIf="errorMessage !== null">{{errorMessage}}</div>`
})
export class WarningMessages {
    @Input() errorType: string = 'Default';
    @Input() errorMsg: string = 'Msg Default';
    constructor() { }

    get errorMessage() {

        if (this.errorMsg === 'Y') {

            let errStr: string;
            if (this.errorType === 'requestor') {
                errStr = 'Warning: Requestor\'s email domain does not match requestor\'s agency\'s email domain';
            }
            else {
                errStr = 'Warning: Consultants\'s email domain does not match Consultants\'s contractor\'s email domain';
            }

            return errStr;
        }
    }
}
