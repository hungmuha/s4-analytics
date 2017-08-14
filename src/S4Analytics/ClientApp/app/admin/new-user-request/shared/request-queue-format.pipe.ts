import { Pipe, PipeTransform } from '@angular/core';
import { NewUserRequestStatus, NewUserRequestType } from './new-user-request-enum';

@Pipe({ name: 'requestStatus' })
export class RequestStatusPipe implements PipeTransform {

    transform(value: number): string {
        switch (value) {
            case NewUserRequestStatus.NewUser: return 'Approve New User';
            case NewUserRequestStatus.NewConsultant: return 'Approve New Consultant';
            case NewUserRequestStatus.NewVendor: return 'Approve New Vendor';
            case NewUserRequestStatus.NewAgency: return 'Approve New Agency';
            case NewUserRequestStatus.CreateAgency: return 'Verify New Agency Created';
            case NewUserRequestStatus.Completed: return 'Completed';
            case NewUserRequestStatus.Rejected: return 'Rejected';
            default: return 'Not implemented' + value;
        }

    }
}

@Pipe({ name: 'requestType' })
export class RequestTypePipe implements PipeTransform {

    transform(value: number): string {
        switch (value) {
            case NewUserRequestType.FlPublicAgencyEmployee: return 'New Employee';
            case NewUserRequestType.FlPublicAgencyMgr: return 'New Consultant';
            default: return 'Not implemented';
        }

    }
}