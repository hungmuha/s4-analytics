import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GridModule } from '@progress/kendo-angular-grid';
import { EventAnalysisComponent } from './event-analysis.component';
import { EventMapComponent } from './event-map.component';
import { CrashService } from './shared';

@NgModule({
    imports: [
        RouterModule,
        CommonModule,
        GridModule
    ],
    declarations: [
        EventAnalysisComponent,
        EventMapComponent
    ],
    providers: [
        CrashService
    ]
})
export class EventAnalysisModule { }
