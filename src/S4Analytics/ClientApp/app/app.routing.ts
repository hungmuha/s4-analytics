import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { EventAnalysisComponent } from './event-analysis';
import { NetworkAnalysisComponent } from './network-analysis';
import { ReportingComponent } from './reporting';
import { TrendAnalysisComponent } from './trend-analysis';
import { AdminComponent, RequestQueueComponent } from './admin';
import { AuthGuard, AnyAdminGuard, Html5ConduitResolve, OptionsResolveService } from './shared';
import { Html5ConduitComponent } from './html5-conduit.component';

export const routes: Routes = [
    {
        path: '',
        component: IndexComponent,
        resolve: {
            options: OptionsResolveService
        },
        children: [
            {
                path: '',
                canActivate: [AuthGuard],
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
                canActivate: [AnyAdminGuard],
                children: [
                    {
                        path: 'request-queue',
                        component: RequestQueueComponent
                    }
                ]
            },
            { path: 'html5-conduit', resolve: { Html5ConduitResolve }, component: Html5ConduitComponent },
            { path: 'login', component: LoginComponent },
            { path: '**', redirectTo: '' }
        ]
    }
];
