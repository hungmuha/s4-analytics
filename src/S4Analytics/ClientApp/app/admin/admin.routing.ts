import { Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { RequestQueueComponent } from './new-user-request/request-queue.component';
import { RequestActionComponent } from './new-user-request/request-action.component';

export const adminRoutes: Routes = [
    {
        path: 'admin',
        component: AdminComponent
    },
    {
        path: '',
        component: AdminComponent,
        children: [
            {
                path: 'admin/request-queue', component: RequestQueueComponent
            }
        ]
    }
];
