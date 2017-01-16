import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { NewUserRequest } from './new-user-request';

@Injectable()
export class NewUserRequestService {
    constructor(private http: Http) { }

    getNewUserRequests(): Observable<NewUserRequest[]> {
        let url = 'api/admin/new-user-request';
        return this.http
            .get(url)
            .map((r: Response) => r.json().data as NewUserRequest[]);
    }
}