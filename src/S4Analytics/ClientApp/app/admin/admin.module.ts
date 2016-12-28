import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { routes } from './admin.routing';
import { AdminComponent } from './admin.component';
import { NewUserRequestMasterComponent } from './new-user-request-master.component';
import { NewUserRequestDetailComponent } from './new-user-request-detail.component';

@NgModule({
    imports: [
        RouterModule.forChild(routes),
        CommonModule,
        FormsModule
    ],
    declarations: [
        AdminComponent,
        NewUserRequestMasterComponent,
        NewUserRequestDetailComponent
    ]
})
export class AdminModule { }