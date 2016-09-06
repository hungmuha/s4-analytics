import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';
import { PbcatViewerComponent } from './pbcat-viewer.component';
import { PbcatResolveService } from './shared';

const pbcatRoutes: Routes = [
    {
        path: 'pbcat/:bikeOrPed/:hsmvReportNumber/step/:stepNumber',
        component: PbcatMasterComponent,
        resolve: { PbcatResolveService }
    },
    {
        path: 'pbcat/:bikeOrPed/:hsmvReportNumber/summary',
        component: PbcatMasterComponent,
        resolve: { PbcatResolveService }
    },
    {
        path: 'pbcat/viewer/:hsmvReportNumber',
        component: PbcatViewerComponent
    }
];

export const routing = RouterModule.forChild(pbcatRoutes);
