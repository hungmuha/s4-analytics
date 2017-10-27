import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReportingComponent } from './reporting.component';
import { CrashesOverTimeComponent } from './crashes-over-time.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule
    ],
    declarations: [
        ReportingComponent,
        CrashesOverTimeComponent
    ]
})
export class ReportingModule { }
