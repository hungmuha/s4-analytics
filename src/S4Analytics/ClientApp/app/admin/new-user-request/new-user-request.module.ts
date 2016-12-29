import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TruncatePipe } from '../../shared';
import { adminRoutes } from '../admin.routing';
import { RequestQueueComponent } from './request-queue.component';
import { RequestActionComponent } from './request-action.component';
import { NewUserRequestStateService } from './shared/new-user-request-state.service';
import { NewUserRequestService } from './shared/new-user-request.service';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NgbModule.forRoot()
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