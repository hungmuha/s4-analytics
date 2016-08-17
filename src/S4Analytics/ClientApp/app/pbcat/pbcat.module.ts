import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PbcatService } from './shared';
import { routing } from './pbcat.routing';
import { PbcatMasterComponent } from './pbcat-master.component';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatItemComponent } from './pbcat-item.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';

@NgModule({
    imports: [
        CommonModule,
        routing
    ],
    declarations: [
        PbcatItemComponent,
        PbcatStepComponent,
        PbcatSummaryComponent,
        PbcatMasterComponent
    ],
    providers: [
        PbcatService
    ]
})
export class PbcatModule { }
