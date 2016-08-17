import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';

const pbcatRoutes: Routes = [
    { path: 'pbcat', redirectTo: 'pbcat/step/1' },
    { path: 'pbcat/:hsmvReportNumber/step/:stepNumber', component: PbcatMasterComponent },
    { path: 'pbcat/:hsmvReportNumber/summary', component: PbcatMasterComponent }
];

export const routing = RouterModule.forChild(pbcatRoutes);
