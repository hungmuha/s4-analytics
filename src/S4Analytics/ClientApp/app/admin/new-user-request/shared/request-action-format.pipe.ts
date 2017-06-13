import { Pipe, PipeTransform } from '@angular/core';
import { NewUserRequestStatus,  } from './new-user-request-enum';

@Pipe({ name: 'approveRejectType' })
export class ApproveRejectTypePipe
    implements PipeTransform {

    transform(value: number): string {
        switch (value) {
            case NewUserRequestStatus.NewUser: return 'Employee';
            case NewUserRequestStatus.NewConsultant: return 'Consultant';
            case NewUserRequestStatus.NewContractor: return 'Vendor';
            case NewUserRequestStatus.NewAgency: return 'Agency';
            case NewUserRequestStatus.CreateAgency: return '???'; //TO BE IMPLEMENTED
            default: return 'Not implemented' + value;
        }
    }
}

@Pipe({ name: 'reportAccess' })
export class ReportAccessPipe
    implements PipeTransform {

    transform(value: boolean): string {
        return value ? 'Before 70 days' : 'After 70 days';
    }
}
