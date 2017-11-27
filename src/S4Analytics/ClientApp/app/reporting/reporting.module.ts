import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ButtonGroupComponent, CardComponent } from '../shared';
import { ReportingComponent } from './reporting.component';
import { CrashesOverTimeComponent } from './crashes-over-time.component';
import { CrashesByYearComponent } from './crashes-by-year.component';
import { CrashesByMonthComponent } from './crashes-by-month.component';
import { CrashesByDayComponent } from './crashes-by-day.component';
import { CrashReportingService } from './shared';
import { CitationsOverTimeComponent } from './citations-over-time.component';
import { CitationsByYearComponent } from './citations-by-year.component';
import { CitationsByMonthComponent } from './citations-by-month.component';
import { CitationReportingService } from './shared';

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
        CrashesByYearComponent,
        CrashesByMonthComponent,
        CrashesByDayComponent,
        CitationsOverTimeComponent,
        CitationsByYearComponent,
        CitationsByMonthComponent
    ],
    providers: [
        CrashReportingService,
        CitationReportingService
    ]
})
export class ReportingModule { }
