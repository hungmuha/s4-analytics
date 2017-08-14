import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AdminComponent } from './admin.component';
import { RequestQueueModule } from './new-user-request/new-user-request.module';

@NgModule({
    imports: [
        RouterModule,
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
