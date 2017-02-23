import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { NewUserRequest } from './new-user-request';
import { NewUserRequestStatus } from './new-user-request-enum';
import { RequestActionResults, NewAgencyActionResults, NewConsultantActionResults } from './request-action-results';

class RequestApproval {
    constructor(
        public requestNumber: number,
        public currentStatus: NewUserRequestStatus,
        public newStatus: NewUserRequestStatus) { }
}

class NewAgencyRequestApproval extends RequestApproval {
    constructor(
        public requestNumber: number,
        public currentStatus: NewUserRequestStatus,
        public newStatus: NewUserRequestStatus,
        public before70Days: boolean,
        public lea: boolean) {
        super(requestNumber, currentStatus, newStatus);
        }
}

class NewConsultantRequestApproval extends RequestApproval {
    constructor(
        public requestNumber: number,
        public currentStatus: NewUserRequestStatus,
        public newStatus: NewUserRequestStatus,
        public before70Days: boolean) {
        super(requestNumber, currentStatus, newStatus);
        }
}



@Injectable()
export class NewUserRequestService {
    constructor(private http: Http) { }

    getNewUserRequests(): Observable<NewUserRequest[]> {
        let url = 'api/admin/new-user-request';
        return this.http
            .get(url)
            .map((r: Response) => r.json() as NewUserRequest[]);
    }

    filterNewUserRequestsBy(s: string): Observable<NewUserRequest[]> {
        let url = `api/admin/new-user-request/filter/${s}`;

        return this.http
            .get(url)
            .map((r: Response) => r.json() as NewUserRequest[]);
    }

    approve(currentStatus: NewUserRequestStatus, requestActionResults: RequestActionResults): Observable<NewUserRequest> {

        let reqWrapper: RequestApproval;

        switch (currentStatus) {
            case NewUserRequestStatus.NewContractor:
                reqWrapper = new RequestApproval(requestActionResults.requestNumber, NewUserRequestStatus.NewContractor, NewUserRequestStatus.NewConsultant);
                break;
            case NewUserRequestStatus.NewAgency:

                reqWrapper = new NewAgencyRequestApproval(
                    requestActionResults.requestNumber,
                    NewUserRequestStatus.NewAgency,
                    NewUserRequestStatus.CreateAgency,
                    (requestActionResults as NewAgencyActionResults).accessBefore70Days,
                    (requestActionResults as NewAgencyActionResults).lea
                );

                break;
            case NewUserRequestStatus.CreateAgency:
                reqWrapper = new RequestApproval(requestActionResults.requestNumber, NewUserRequestStatus.CreateAgency, NewUserRequestStatus.NewUser);
                break;
            case NewUserRequestStatus.NewConsultant:

                reqWrapper = new NewConsultantRequestApproval(
                    requestActionResults.requestNumber,
                    NewUserRequestStatus.NewConsultant,
                    NewUserRequestStatus.Completed,
                    (requestActionResults as NewConsultantActionResults).accessBefore70Days
                );

                break;
            default:
                reqWrapper = new RequestApproval(requestActionResults.requestNumber, currentStatus.valueOf(), NewUserRequestStatus.Completed);
                break;
        }

        let approveType = this.getApproveType(currentStatus);
        let url = `api/admin/new-user-request/${requestActionResults.requestNumber}/approve/${approveType}`;

        return this.http
            .patch(url, reqWrapper)
            .map(res => res.json())
            .catch(this.handleError);
    }

    private handleError(error: any) {
        let errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg);
        return Observable.throw(errMsg);
    }

    private getApproveType(status: NewUserRequestStatus): 'agency' | 'consultant' | '' {
        switch (status) {
            case NewUserRequestStatus.NewAgency:
                return 'agency';
            case NewUserRequestStatus.NewConsultant:
                return 'consultant';
            default:
                return '';
        }
    }
}