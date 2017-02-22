import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { adminRoutes } from './admin.routing';
import { AdminComponent } from './admin.component';
import { RequestQueueModule } from './new-user-request/new-user-request.module';

@NgModule({
    imports: [
        RouterModule.forChild(adminRoutes),
        CommonModule,
        FormsModule,
        RequestQueueModule,
        NgbModule
    ],
    declarations: [
        AdminComponent
    ]
})
export class AdminModule { }