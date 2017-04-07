import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { AnalyticsComponent } from './analytics.component';
import { EventAnalysisComponent } from './event-analysis/event-analysis.component';
import { NetworkAnalysisComponent } from './network-analysis/network-analysis.component';
import { ReportingComponent } from './reporting/reporting.component';
import { TrendAnalysisComponent } from './trend-analysis/trend-analysis.component';
import { AdminComponent } from './admin/admin.component';
import { RequestQueueComponent } from './admin/new-user-request/request-queue.component';

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
