﻿import { Injectable } from '@angular/core';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { NewUserRequest } from './new-user-request';
import { RequestActionResults } from './request-action-results';


@Injectable()
export class NewUserRequestStateService {
    newUserRequests: NewUserRequest[];
    selectedRequest: NewUserRequest;
    currentRequestActionResults: RequestActionResults;
    contractViewerWindow: Window;
    sortField: string[] = ['requestDt'];
    sortAsc: boolean = true;
    sortColumn: number;
    currentActionForm: NgbModalRef;
}