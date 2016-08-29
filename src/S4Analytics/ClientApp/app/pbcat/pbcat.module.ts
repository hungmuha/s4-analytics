import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PbcatService } from './shared';
import { routing } from './pbcat.routing';
import { PbcatMasterComponent } from './pbcat-master.component';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';
import { PbcatResolve } from './shared';

@NgModule({
    imports: [
        CommonModule,
        routing
    ],
    declarations: [
        PbcatStepComponent,
        PbcatSummaryComponent,
        PbcatMasterComponent
    ],
    providers: [
        PbcatService,
        PbcatResolve
    ]
})
export class PbcatModule { }
