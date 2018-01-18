import { NgModule } from '@angular/core';
import { S4CommonModule } from '../s4-common.module';
import { CrashReportingService, CitationReportingService } from './shared';
import { ReportingComponent } from './reporting.component';
import { CrashesOverTimeComponent } from './crashes-over-time.component';
import { CitationsOverTimeComponent } from './citations-over-time.component';
import { EventsByYearComponent } from './events-by-year.component';
import { EventsByMonthComponent } from './events-by-month.component';
import { EventsByDayComponent } from './events-by-day.component';
import { EventsByAttributeComponent } from './events-by-attribute.component';
import { DataTimelinessComponent } from './data-timeliness.component';

@NgModule({
    imports: [
        S4CommonModule
    ],
    declarations: [
        ReportingComponent,
        CrashesOverTimeComponent,
        CitationsOverTimeComponent,
        EventsByYearComponent,
        EventsByMonthComponent,
        EventsByDayComponent,
        EventsByAttributeComponent,
        DataTimelinessComponent
    ],
    providers: [
        CrashReportingService,
        CitationReportingService
    ]
})
export class ReportingModule { }
