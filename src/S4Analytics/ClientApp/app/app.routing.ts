import { Routes, RouterModule } from '@angular/router';
import { IndexComponent } from './index.component';
import { ReportViewerComponent } from './report-viewer.component';
import { OptionsResolveService } from './options-resolve.service';

const appRoutes: Routes = [
    { path: '', component: IndexComponent, pathMatch: 'full' },
    {
        path: 'report-viewer/:hsmvReportNumber',
        component: ReportViewerComponent,
        resolve: {
            options: OptionsResolveService
        }
    },
    { path: '**', redirectTo: '' }
];

export const appRoutingProviders: any[] = [];

export const routing = RouterModule.forRoot(appRoutes);
