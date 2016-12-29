import { Injectable } from '@angular/core';
import { NewUserRequest } from './new-user-request';

@Injectable()
export class NewUserRequestStateService {
    newUserRequests: NewUserRequest[];
}