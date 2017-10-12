import { NgModule  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
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
        CommonModule,
        FormsModule,
        NgbModule
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
        NewUserRequestStateService,
        DatePipe
    ]
})
export class RequestQueueModule { }