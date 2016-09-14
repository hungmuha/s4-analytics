import { Routes, RouterModule } from '@angular/router';
import { PbcatFlowComponent } from './pbcat-flow.component';
import { PbcatResolveService } from './shared';

const pbcatRoutes: Routes = [
    {
        path: 'pbcat/:hsmvReportNumber/step/:stepNumber',
        component: PbcatFlowComponent,
        resolve: { PbcatResolveService }
    },
    {
        path: 'pbcat/:hsmvReportNumber/summary',
        component: PbcatFlowComponent,
        resolve: { PbcatResolveService }
    }
];

export const routing = RouterModule.forChild(pbcatRoutes);
