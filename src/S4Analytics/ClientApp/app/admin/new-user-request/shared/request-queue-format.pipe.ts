import { Pipe, PipeTransform } from '@angular/core';
import { NewUserRequestStatus, NewUserRequestType } from './new-user-request-enum';

@Pipe({ name: 'requestStatus' })
export class RequestStatusPipe implements PipeTransform {

    newUserRequestStatus = NewUserRequestStatus;

    transform(value: string): string {
        switch (value) {
            case this.newUserRequestStatus[NewUserRequestStatus.NewUser]: return 'Approve New User';
            case this.newUserRequestStatus[NewUserRequestStatus.NewConsultant]: return 'Approve New Consultant';
            case this.newUserRequestStatus[NewUserRequestStatus.NewContractorAndConsultant]: return 'Approve New Contractor and Consultant';
            default: return 'Not implemented';
        }

    }
}

@Pipe({ name: 'requestType' })
export class RequestTypePipe implements PipeTransform {

    newUserRequestType = NewUserRequestType;

    transform(value: string): string {
        switch (value) {
            case this.newUserRequestType[NewUserRequestType.FlPublicAgencyEmployee]: return 'Florida Public Agency Employee';
            case this.newUserRequestType[NewUserRequestType.FlPublicAgencyMgr]: return 'Florida Public Agency Manager';
            default: return 'Not implemented';
        }

    }
}