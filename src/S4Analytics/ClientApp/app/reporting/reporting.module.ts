import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReportingComponent } from './reporting.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule
    ],
    declarations: [
        ReportingComponent
    ]
})
export class ReportingModule { }
