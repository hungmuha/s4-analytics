import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TruncatePipe } from '../shared';
import { PbcatService } from './shared';
import { routes } from './pbcat.routing';
import { PbcatFlowComponent } from './pbcat-flow.component';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';
import { PbcatResolveService, PbcatStateService } from './shared';

@NgModule({
    imports: [
        RouterModule.forChild(routes),
        CommonModule,
        FormsModule
    ],
    declarations: [
        TruncatePipe,
        PbcatStepComponent,
        PbcatSummaryComponent,
        PbcatFlowComponent
    ],
    providers: [
        PbcatService,
        PbcatResolveService,
        PbcatStateService
    ]
})
export class PbcatModule { }
