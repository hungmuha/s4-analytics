import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './app/home';
import { PbcatComponent } from './app/pbcat';

export const appRoutes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'pbcat', redirectTo: 'pbcat/step/1' },
    { path: 'pbcat/:hsmvReportNumber/step/:stepNumber', component: PbcatComponent },
    { path: 'pbcat/:hsmvReportNumber/summary', component: PbcatComponent },
    { path: '**', redirectTo: 'home' }
];

export const appRoutingProviders: any[] = [];

export const routing = RouterModule.forRoot(appRoutes);
