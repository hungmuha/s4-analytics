import { Injectable } from '@angular/core';
import { NewUserRequest } from './new-user-request';
import { RequestActionResults } from './request-action-results';

@Injectable()
export class NewUserRequestStateService {
    newUserRequests: NewUserRequest[];

    selectedRequest: NewUserRequest;
    currentRequestActionResults: RequestActionResults;

    public get isRejected(): boolean {

        if (!this.currentRequestActionResults === undefined) {
            console.log('approved: undefined');
            return true;
        }

        console.log('approved: ' + this.currentRequestActionResults.approved);

        return !this.currentRequestActionResults.approved;
    }

}