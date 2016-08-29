import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';
import { PbcatResolve } from './shared';

const pbcatRoutes: Routes = [
    {
        path: 'pbcat/:bikeOrPed/:hsmvReportNumber/step/:stepNumber',
        component: PbcatMasterComponent,
        resolve: { PbcatResolve }
    },
    {
        path: 'pbcat/:bikeOrPed/:hsmvReportNumber/summary',
        component: PbcatMasterComponent,
        resolve: { PbcatResolve }
    }
];

export const routing = RouterModule.forChild(pbcatRoutes);
