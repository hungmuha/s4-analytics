import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { ReportViewerComponent } from './report-viewer.component';

export const routes: Routes = [
    { path: '', component: IndexComponent, pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'report-viewer/:hsmvReportNumber', component: ReportViewerComponent },
    { path: '**', redirectTo: '' }
];
