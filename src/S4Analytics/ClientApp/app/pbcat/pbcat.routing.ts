import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';

const pbcatRoutes: Routes = [
    { path: 'pbcat/:hsmvReportNumber/step/:stepNumber', component: PbcatMasterComponent },
    { path: 'pbcat/:hsmvReportNumber/summary', component: PbcatMasterComponent }
];

export const routing = RouterModule.forChild(pbcatRoutes);
