import { NgModule } from '@angular/core';
import { S4CommonModule } from '../s4-common.module';
import { AdminComponent } from './admin.component';
import { RequestQueueModule } from './new-user-request/new-user-request.module';

@NgModule({
    imports: [
        S4CommonModule,
        RequestQueueModule
    ],
    declarations: [
        AdminComponent
    ]
})
export class AdminModule { }
