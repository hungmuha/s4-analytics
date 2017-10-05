import { Injectable } from '@angular/core';
import { NewUserRequest } from './new-user-request';
import { RequestActionResults } from './request-action-results';
import { QueueFilter } from './new-user-request-enum';
import { CurrentActionForm } from './current-action-form';


@Injectable()
export class NewUserRequestStateService {
    newUserRequests: NewUserRequest[];
    selectedRequest: NewUserRequest;
    isSelectedRequestLocked: boolean;
    currentRequestActionResults: RequestActionResults;
    contractViewerWindow?: Window;
    sortAsc: boolean = true;
    sortColumnName: string = 'requestNbr';
    queueFilter: QueueFilter = QueueFilter.Pending;
    currentActionForm: CurrentActionForm = new CurrentActionForm();
    requestorWarningMessages: string[];
    consultantWarningMessages: string[];
    warningMessages: string[];
}