import { NgModule } from '@angular/core';
import { S4CommonModule } from '../../s4-common.module';
import { RequestQueueComponent } from './request-queue.component';
import { RequestActionComponent } from './request-action.component';
import { NewEmployeeComponent } from './new-employee.component';
import { NewAgencyComponent } from './new-agency.component';
import { AgencyCreateComponent } from './agency-create.component';
import { NewConsultantComponent } from './new-consultant.component';
import { RequestWarningComponent } from './request-warning.component';
import {
    NewUserRequestStateService, NewUserRequestService, RequestStatusPipe, RequestTypePipe,
    OrderByPipe, ApproveRejectTypePipe, ReportAccessPipe
} from './shared';

@NgModule({
    imports: [
        S4CommonModule,
    ],
    declarations: [
        RequestQueueComponent,
        RequestActionComponent,
        NewEmployeeComponent,
        NewAgencyComponent,
        AgencyCreateComponent,
        NewConsultantComponent,
        RequestWarningComponent,
        RequestStatusPipe,
        RequestTypePipe,
        OrderByPipe,
        ApproveRejectTypePipe,
        ReportAccessPipe
    ],
    providers: [
        NewUserRequestService,
        NewUserRequestStateService
    ]
})
export class RequestQueueModule { }
