import { Routes } from '@angular/router';
import { PbcatFlowComponent } from './pbcat-flow.component';
import { PbcatResolveService } from './shared';

export const routes: Routes = [
    {
        path: 'pbcat/:hsmvReportNumber/step/:stepNumber',
        component: PbcatFlowComponent,
        resolve: {
            flow: PbcatResolveService
        }
    },
    {
        path: 'pbcat/:hsmvReportNumber/summary',
        component: PbcatFlowComponent,
        resolve: {
            flow: PbcatResolveService
        }
    }
];
