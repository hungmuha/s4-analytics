import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TruncatePipe } from '../shared';
import { adminRoutes } from './admin.routing';
import { AdminComponent } from './admin.component';
import { NewUserRequestStateService, NewUserRequestService } from './shared';
import { RequestQueueModule } from './new-user-request/new-user-request.module';

@NgModule({
    imports: [
        RouterModule.forRoot(adminRoutes),
        CommonModule,
        FormsModule,
        NgbModule.forRoot(),
        RequestQueueModule
    ],
    declarations: [
        AdminComponent
    ],
    providers: [
        NewUserRequestService,
        NewUserRequestStateService
    ]
})
export class AdminModule { }