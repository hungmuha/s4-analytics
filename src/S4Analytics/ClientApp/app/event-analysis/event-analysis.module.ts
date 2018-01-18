import { NgModule } from '@angular/core';
import { S4CommonModule } from '../s4-common.module';
import { CrashService, EventAnalysisStateService } from './shared';
import { EventAnalysisComponent } from './event-analysis.component';
import { EventMapComponent } from './event-map.component';

@NgModule({
    imports: [
        S4CommonModule
    ],
    declarations: [
        EventAnalysisComponent,
        EventMapComponent
    ],
    providers: [
        CrashService,
        EventAnalysisStateService
    ]
})
export class EventAnalysisModule { }
