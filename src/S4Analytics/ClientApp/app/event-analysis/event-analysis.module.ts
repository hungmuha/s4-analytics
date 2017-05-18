import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CrashService } from './shared';
import { EventAnalysisComponent } from './event-analysis.component';
import { EventMapComponent } from './event-map.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule
    ],
    declarations: [
        EventAnalysisComponent,
        EventMapComponent
    ],
    providers: [
        CrashService
    ]
})
export class EventAnalysisModule { }
