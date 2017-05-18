import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { AnalyticsComponent } from './analytics.component';
import { EventAnalysisComponent, KendoPocComponent, NgxDatatablePocComponent, AgGridPocComponent } from './event-analysis';
import { NetworkAnalysisComponent } from './network-analysis';
import { ReportingComponent } from './reporting';
import { TrendAnalysisComponent } from './trend-analysis';
import { AdminComponent, RequestQueueComponent } from './admin';

export const routes: Routes = [
    {
        path: '',
        component: IndexComponent,
        children: [
            {
                path: '',
                component: AnalyticsComponent,
                children: [
                    { path: '', redirectTo: 'event', pathMatch: 'full' },
                    { path: 'event', component: EventAnalysisComponent },
                    { path: 'kendo-poc', component: KendoPocComponent },
                    { path: 'ngx-datatable-poc', component: NgxDatatablePocComponent },
                    { path: 'ag-grid-poc', component: AgGridPocComponent },
                    { path: 'network', component: NetworkAnalysisComponent },
                    { path: 'reporting', component: ReportingComponent },
                    { path: 'trend', component: TrendAnalysisComponent }
                ]
            },
            {
                path: 'admin',
                component: AdminComponent,
                children: [
                    { path: 'request-queue', component: RequestQueueComponent }
                ]
            }
        ]
    },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: '' }
];
