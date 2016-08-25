import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';

const pbcatRoutes: Routes = [
    { path: 'pbcat/:bikeOrPed/:hsmvReportNumber/step/:stepNumber', component: PbcatMasterComponent },
    { path: 'pbcat/:bikeOrPed/:hsmvReportNumber/summary', component: PbcatMasterComponent }
];

export const routing = RouterModule.forChild(pbcatRoutes);
