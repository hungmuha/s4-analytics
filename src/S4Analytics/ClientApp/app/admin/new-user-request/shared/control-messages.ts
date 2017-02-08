import { Component, Input } from "@angular/core";
//import { ValidationService } from "./validation.service";
/**
 * Created by rob on 6/16/16.
 */


// control-message component from https://coryrylan.com/blog/angular-2-form-builder-and-validation-management
@Component({
    selector: 'control-messages',
    template: `<div style="color:red" *ngIf="errorMessage !== null">{{errorMessage}}</div>`
})
export class ControlMessages {
    @Input() errorType: string = 'Default';
    @Input() errorMsg: string = 'Msg Default';
    constructor() { }

    get errorMessage() {

        if (this.errorMsg == 'Y') {

            let errStr: string;
            if (this.errorType == 'requestor') {
                errStr = 'Warning: Requestor\'s email domain does not match requestor\'s agency\'s email domain';
            }
            else {
                errStr = 'Warning: Consultants\'s email domain does not match Consultants\'s contractor\'s email domain';
            }

            return errStr;
        }
    }
}
