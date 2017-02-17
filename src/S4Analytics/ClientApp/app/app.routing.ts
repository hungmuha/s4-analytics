import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';

export const routes: Routes = [
    { path: '', component: IndexComponent, pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: '' }
];
