import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TruncatePipe } from '../shared';
import { PbcatService } from './shared';
import { routing } from './pbcat.routing';
import { PbcatFlowComponent } from './pbcat-flow.component';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';
import { PbcatResolveService, PbcatStateService } from './shared';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        routing
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
