import { Routes } from '@angular/router';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { EventAnalysisComponent } from './event-analysis';
import { ReportingComponent, CrashesOverTimeComponent, CitationsOverTimeComponent } from './reporting';
import { AdminComponent, RequestQueueComponent } from './admin';
import { AuthGuard, AnyAdminGuard, Html5ConduitResolve, OptionsResolveService, ServerDateResolveService } from './shared';
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
                    { path: '', redirectTo: 'reporting/crashes-over-time', pathMatch: 'full' },
                    { path: 'event', resolve: { serverDate: ServerDateResolveService }, component: EventAnalysisComponent },
                    {
                        path: 'reporting',
                        component: ReportingComponent,
                        children: [
                            { path: '', redirectTo: 'crashes-over-time', pathMatch: 'full' },
                            { path: 'crashes-over-time', component: CrashesOverTimeComponent },
                            { path: 'citations-over-time', component: CitationsOverTimeComponent }
                        ]
                    }
                ]
            },
            {
                path: 'admin',
                component: AdminComponent,
                canActivate: [AnyAdminGuard],
                children: [
                    { path: '', redirectTo: 'request-queue', pathMatch: 'full' },
                    { path: 'request-queue', component: RequestQueueComponent }
                ]
            },
            { path: 'html5-conduit', resolve: { Html5ConduitResolve }, component: Html5ConduitComponent },
            { path: 'login', component: LoginComponent },
            { path: '**', redirectTo: '' }
        ]
    }
];
