import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ButtonGroupComponent } from '../shared';
import { ReportingComponent } from './reporting.component';
import { CrashesOverTimeComponent } from './crashes-over-time.component';
import { CrashesByYearComponent } from './crashes-by-year.component';
import { CrashesByMonthComponent } from './crashes-by-month.component';
import { CrashesByDayComponent } from './crashes-by-day.component';
import { ReportingService } from './shared';

@NgModule({
    imports: [
        RouterModule,
        CommonModule,
        FormsModule
    ],
    declarations: [
        ButtonGroupComponent,
        ReportingComponent,
        CrashesOverTimeComponent,
        CrashesByYearComponent,
        CrashesByMonthComponent,
        CrashesByDayComponent
    ],
    providers: [
        ReportingService
    ]
})
export class ReportingModule { }
