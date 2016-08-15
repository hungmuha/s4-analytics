import { RouterConfig } from '@angular/router';
import { HomeComponetn } from './app/home';
import { PbcatComponent } from './app/pbcat';

export const routes: RouterConfig = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponetn },
    { path: 'pbcat', redirectTo: 'pbcat/step/1' },
    { path: 'pbcat/step/:currentStep', component: PbcatComponent },
    { path: 'pbcat/summary', component: PbcatComponent },
    { path: '**', redirectTo: 'home' }
];
