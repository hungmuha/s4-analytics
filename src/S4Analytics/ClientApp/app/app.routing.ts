import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { AnalyticsComponent } from './analytics.component';
import { EventAnalysisComponent } from './event-analysis';
import { NetworkAnalysisComponent } from './network-analysis';
import { ReportingComponent } from './reporting';
import { TrendAnalysisComponent } from './trend-analysis';
import { AdminComponent, RequestQueueComponent } from './admin';
import { AuthGuard, AnyAdminGuard } from './shared';

export const routes: Routes = [
    {
        path: '',
        component: IndexComponent,
        children: [
            {
                path: '',
                component: AnalyticsComponent,
                canActivate: [AuthGuard],
                children: [
                    // { path: '', redirectTo: 'event', pathMatch: 'full' },
                    { path: 'event', component: EventAnalysisComponent },
                    { path: 'network', component: NetworkAnalysisComponent },
                    { path: 'reporting', component: ReportingComponent },
                    { path: 'trend', component: TrendAnalysisComponent }
                ]
            },
            {
                path: 'admin',
                component: AdminComponent,
                canActivate: [AnyAdminGuard],
                children: [
                    {
                        path: 'request-queue',
                        component: RequestQueueComponent
                    }
                ]
            }
        ]
    },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: '' }
];
