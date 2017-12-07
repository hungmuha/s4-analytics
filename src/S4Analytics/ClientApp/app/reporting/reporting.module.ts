import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ButtonGroupComponent, CardComponent } from '../shared';
import { ReportingComponent } from './reporting.component';
import { CrashesOverTimeComponent } from './crashes-over-time.component';
import { CitationsOverTimeComponent } from './citations-over-time.component';
import { EventsByYearComponent } from './events-by-year.component';
import { EventsByMonthComponent } from './events-by-month.component';
import { EventsByDayComponent } from './events-by-day.component';
import { EventsByAttributeComponent } from './events-by-attribute.component';
import { DataTimelinessComponent } from './data-timeliness.component';
import { CrashReportingService, CitationReportingService } from './shared';

@NgModule({
    imports: [
        RouterModule,
        CommonModule,
        FormsModule,
        NgbModule
    ],
    declarations: [
        ButtonGroupComponent,
        CardComponent,
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
