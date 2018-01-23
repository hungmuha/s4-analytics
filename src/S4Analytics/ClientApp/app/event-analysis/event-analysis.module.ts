import { NgModule } from '@angular/core';
import { S4CommonModule } from '../s4-common.module';
import { CrashService, EventAnalysisStateService } from './shared';
import { EventAnalysisComponent } from './event-analysis.component';
import { EventMapComponent } from './event-map.component';
import { DateTimeModalComponent } from './date-time-modal.component';

@NgModule({
    imports: [
        S4CommonModule
    ],
    declarations: [
        EventAnalysisComponent,
        EventMapComponent,
        DateTimeModalComponent
    ],
    providers: [
        CrashService,
        EventAnalysisStateService
    ]
})
export class EventAnalysisModule { }
