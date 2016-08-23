import { Routes, RouterModule } from '@angular/router';
import { ErrorComponent } from './error.component';

const appRoutes: Routes = [
    { path: '', redirectTo: 'pbcat/12345678/step/1', pathMatch: 'full' },
    { path: 'error', component: ErrorComponent },
    { path: '**', redirectTo: 'pbcat/12345678/step/1' }
];

export const appRoutingProviders: any[] = [];

export const routing = RouterModule.forRoot(appRoutes);
