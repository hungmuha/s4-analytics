import { Routes, RouterModule } from '@angular/router';
import { PbcatMasterComponent } from './pbcat-master.component';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';

const pbcatRoutes: Routes = [
    {
        path: 'pbcat/:hsmvReportNumber',
        component: PbcatMasterComponent,
        children: [
            { path: 'step/:stepNumber', component: PbcatStepComponent},
            { path: 'summary', component: PbcatSummaryComponent }
        ]
    }
];

export const routing = RouterModule.forChild(pbcatRoutes);
