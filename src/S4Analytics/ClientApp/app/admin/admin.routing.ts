import { Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { RequestQueueComponent } from './new-user-request/request-queue.component';

export const adminRoutes: Routes = [
    {
        path: 'admin',
        component: AdminComponent,
        children: [
            {
                path: 'request-queue',
                component: RequestQueueComponent
            }
        ]
    }
];
