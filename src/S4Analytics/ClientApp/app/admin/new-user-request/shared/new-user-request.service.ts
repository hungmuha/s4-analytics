import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { NewUserRequest } from './new-user-request';
import { NewUserRequestType, NewUserRequestStatus } from './new-user-request-enum';

@Injectable()
export class NewUserRequestService {
    constructor(private http: Http) { }

    getNewUserRequests(): Observable<NewUserRequest[]> {
        let url = 'api/admin/new-user-request';
        return this.http
            .get(url)
            .map((r: Response) => r.json().data as NewUserRequest[]);
    }

    getNewUserRequestsTemp(): NewUserRequest[] {

        let nur1 = {
                requestNbr: 123,
                requestDt: new Date('12/16/2017'),
                requestDesc: 'New Employee',
                requestType: NewUserRequestType.FlPublicAgencyEmployee,
                requestStatus: NewUserRequestStatus.NewUser,
                agncyId: 7,
                requestorFirstNm: 'Sara',
                requestorLastNm: 'Yorty',
                requestorEmail: 'syorty@alachua.gov',
                contractorId: 0,
                warnRequestorEmailCd: 'N',
                warnConsultantEmailCd: 'N',
            };

        let nur2 = {
                requestNbr: 123,
                requestDt: new Date('12/18/2017'),
                requestDesc: 'New Contractor, New Consultant',
                requestType: NewUserRequestType.FlPublicAgencyMgr,
                requestStatus: NewUserRequestStatus.NewContractorAndConsultant,
                agncyId: 12,
                requestorFirstNm: 'Jim',
                requestorLastNm: 'Jones',
                requestorEmail: 'jjones@alachua.gov',
                newContractorNm: 'New Contract Company, Inc',
                newContractorEmailDomain: 'ncc.gov',
                consultantFirstNm: 'Mary',
                consultantLastNm: 'Smith',
                consultantEmail: 'msmith@ncc.gov',
                accessReasonTx: 'She needs to do some work',
                warnRequestorEmailCd: 'N',
                warnConsultantEmailCd: 'N',
            };

        return [nur1 as NewUserRequest, nur2 as NewUserRequest];
    }




}