﻿import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TruncatePipe } from '../shared';
import { adminRoutes } from './admin.routing';
import { AdminComponent } from './admin.component';
import { RequestQueueComponent } from './new-user-request/request-queue.component';
import { RequestActionComponent } from './new-user-request/request-action.component';

@NgModule({
    imports: [
        RouterModule.forRoot(adminRoutes),
        CommonModule,
        FormsModule
    ],
    declarations: [
        AdminComponent,
        RequestQueueComponent,
        RequestActionComponent
    ]
})
export class AdminModule { }