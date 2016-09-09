import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';
import { PbcatViewerComponent } from './pbcat-viewer.component';
import { PbcatResolveService } from './shared';

const pbcatRoutes: Routes = [
    {
        path: 'pbcat/:hsmvReportNumber/step/:stepNumber',
        component: PbcatMasterComponent,
        resolve: { PbcatResolveService }
    },
    {
        path: 'pbcat/:hsmvReportNumber/summary',
        component: PbcatMasterComponent,
        resolve: { PbcatResolveService }
    },
    {
        path: 'pbcat/viewer/:hsmvReportNumber',
        component: PbcatViewerComponent
    }
];

export const routing = RouterModule.forChild(pbcatRoutes);
