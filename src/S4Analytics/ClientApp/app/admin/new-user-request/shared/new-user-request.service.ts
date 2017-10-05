import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { NewUserRequest } from './new-user-request';
import { NewUserRequestStatus } from './new-user-request-enum';
import { RequestActionResults } from './request-action-results';

class RequestApproval {
    constructor(
        public requestNumber: number,
        public selectedRequest: NewUserRequest,
        public currentStatus: NewUserRequestStatus,
        public newStatus: NewUserRequestStatus) { }
}

class NewAgencyRequestApproval extends RequestApproval {
    constructor(
        public requestNumber: number,
        public selectedRequest: NewUserRequest,
        public currentStatus: NewUserRequestStatus,
        public newStatus: NewUserRequestStatus,
        public before70Days: boolean
       // ,        public lea?: boolean
        ) {
        super(requestNumber, selectedRequest, currentStatus, newStatus);
        }
}

class NewConsultantVendorRequestApproval extends RequestApproval {
    constructor(
        public requestNumber: number,
        public selectedRequest: NewUserRequest,
        public currentStatus: NewUserRequestStatus,
        public newStatus: NewUserRequestStatus,
        public before70Days: boolean,
        public contractEndDt: string
        ) {
        super(requestNumber, selectedRequest, currentStatus, newStatus);
        }
}

class RequestRejection {
    constructor(
        public requestNumber: number,
        public selectedRequest: NewUserRequest,
        public rejectionReason: string,
        public newStatus: NewUserRequestStatus
       ) { }
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

    approve(selectedRequest: NewUserRequest,
        requestActionResults: RequestActionResults): Observable<NewUserRequest> {

        let currentStatus = selectedRequest.requestStatus;
        let reqWrapper: RequestApproval;

        switch (currentStatus) {
            case NewUserRequestStatus.NewVendor:
                reqWrapper = new NewConsultantVendorRequestApproval(
                    requestActionResults.requestNumber,
                    selectedRequest,
                    NewUserRequestStatus.NewVendor,
                    NewUserRequestStatus.NewConsultant,
                    requestActionResults.accessBefore70Days,
                    requestActionResults.contractEndDt);
                break;
            case NewUserRequestStatus.NewAgency:

                reqWrapper = new NewAgencyRequestApproval(
                    requestActionResults.requestNumber,
                    selectedRequest,
                    NewUserRequestStatus.NewAgency,
                    NewUserRequestStatus.CreateAgency,
                    requestActionResults.accessBefore70Days
                    //,requestActionResults.lea
                );

                break;
            case NewUserRequestStatus.CreateAgency:
                reqWrapper = new RequestApproval(
                    requestActionResults.requestNumber,
                    selectedRequest,
                    NewUserRequestStatus.CreateAgency,
                    NewUserRequestStatus.NewUser);
                break;
            case NewUserRequestStatus.NewConsultant:

                reqWrapper = new NewConsultantVendorRequestApproval(
                    requestActionResults.requestNumber,
                    selectedRequest,
                    NewUserRequestStatus.NewConsultant,
                    NewUserRequestStatus.Completed,
                    requestActionResults.accessBefore70Days,
                    requestActionResults.contractEndDt
                );
                break;
            default:
                reqWrapper = new RequestApproval(
                    requestActionResults.requestNumber,
                    selectedRequest,
                    currentStatus.valueOf(),
                    NewUserRequestStatus.Completed);
                break;
        }

        let approveType = this.getApproveType(currentStatus);
        let url = `api/admin/new-user-request/${requestActionResults.requestNumber}/approve/${approveType}`;

        return this.http
            .patch(url, reqWrapper)
            .map(res =>  res.json())
            .catch((error: any) => this.handleError(error));
    }

    reject(requestActionResults: RequestActionResults, selectedRequest: NewUserRequest): Observable<NewUserRequest> {
        let reqWrapper: RequestRejection;

        reqWrapper = new RequestRejection(
            requestActionResults.requestNumber,
            selectedRequest,
            requestActionResults.rejectionReason,
            NewUserRequestStatus.Rejected);

        let url = `api/admin/new-user-request/${requestActionResults.requestNumber}/reject`;

        return this.http
            .patch(url, reqWrapper)
            .map(res => res.json())
            .catch(this.handleError);
    }

    doesAgencyExist(agencyNm: string) {
        let encodedAgencyNm = encodeURI(agencyNm);
        let url = `api/admin/new-user-request/${encodedAgencyNm}/verify-agency`;
        return this.http
            .get(url)
            .map((r: Response) => r.json() as boolean);
    }

    private handleError(error: any) {
        let errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        return Observable.throw(errMsg);
    }

    private getApproveType(status: NewUserRequestStatus): 'agency' | 'consultant' | 'vendor' |'' {
        switch (status) {
            case NewUserRequestStatus.NewAgency:
                return 'agency';
            case NewUserRequestStatus.NewConsultant:
                return 'consultant';
            case NewUserRequestStatus.NewVendor:
                return 'vendor';
            default:
                return '';
        }
    }
}