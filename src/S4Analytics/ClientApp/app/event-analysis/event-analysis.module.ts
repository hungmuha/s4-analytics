import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { EventAnalysisComponent } from './event-analysis.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule
    ],
    declarations: [
        EventAnalysisComponent
    ]
})
export class EventAnalysisModule { }
