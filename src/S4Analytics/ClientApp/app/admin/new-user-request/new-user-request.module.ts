import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { RequestQueueComponent } from './request-queue.component';
import { NewEmployeeComponent } from './new-employee.component';
import { NewConsultantComponent } from './new-consultant.component';
import { NewContractorComponent } from './new-contractor.component';
import { NewNonFlComponent } from './non-fl-employee.component';

import { NewUserRequestStateService, NewUserRequestService } from './shared';

@NgModule({
    imports: [
        CommonModule,
        FormsModule
    ],
    declarations: [
        RequestQueueComponent,
        NewEmployeeComponent,
        NewConsultantComponent,
        NewContractorComponent,
        NewNonFlComponent
    ],
    providers: [
        NewUserRequestService,
        NewUserRequestStateService
    ]
})
export class RequestQueueModule { }