import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PbcatService } from './shared';
import { routing } from './pbcat.routing';
import { PbcatFlowComponent } from './pbcat-flow.component';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';
import { PbcatResolveService } from './shared';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        routing
    ],
    declarations: [
        PbcatStepComponent,
        PbcatSummaryComponent,
        PbcatFlowComponent
    ],
    providers: [
        PbcatService,
        PbcatResolveService
    ]
})
export class PbcatModule { }
