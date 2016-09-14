import { Routes, RouterModule } from '@angular/router';
import { IndexComponent } from './index.component';
import { ErrorComponent } from './error.component';
import { ReportViewerComponent } from './report-viewer.component';

const appRoutes: Routes = [
    { path: '', component: IndexComponent, pathMatch: 'full' },
    { path: 'error', component: ErrorComponent },
    {
        path: 'report-viewer/:hsmvReportNumber',
        component: ReportViewerComponent
    },
    { path: '**', redirectTo: '' }
];

export const appRoutingProviders: any[] = [];

export const routing = RouterModule.forRoot(appRoutes);
