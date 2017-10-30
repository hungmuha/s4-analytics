import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReportingComponent } from './reporting.component';
import { CrashesOverTimeComponent } from './crashes-over-time.component';
import { ReportingService } from './shared';

@NgModule({
    imports: [
        RouterModule,
        CommonModule
    ],
    declarations: [
        ReportingComponent,
        CrashesOverTimeComponent
    ],
    providers: [
        ReportingService
    ]
})
export class ReportingModule { }
