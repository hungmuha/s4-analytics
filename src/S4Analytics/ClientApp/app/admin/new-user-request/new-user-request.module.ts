import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RequestQueueComponent } from './request-queue.component';
import { RequestActionComponent } from './request-action.component';
import { NewUserRequestStateService, NewUserRequestService } from '../shared';

@NgModule({
    imports: [
        CommonModule,
        FormsModule
    ],
    declarations: [
        RequestQueueComponent,
        RequestActionComponent
    ],
    providers: [
        NewUserRequestService,
        NewUserRequestStateService
    ]
})
export class RequestQueueModule { }