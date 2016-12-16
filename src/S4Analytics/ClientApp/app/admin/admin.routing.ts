import { Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { NewUserRequestMasterComponent } from './new-user-request-master.component';
import { NewUserRequestDetailComponent } from './new-user-request-detail.component';

export const routes: Routes = [
    {
        path: 'admin',
        component: AdminComponent
    },
    {
        path: 'newuserrequestqueue',
        component: NewUserRequestMasterComponent
    },
    {
        path: 'newuserrequestprocess',
        component: NewUserRequestDetailComponent
    }

];
