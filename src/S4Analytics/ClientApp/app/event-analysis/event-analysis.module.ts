import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GridModule } from '@progress/kendo-angular-grid';
import { CrashService } from './shared';
import { EventAnalysisComponent } from './event-analysis.component';
import { EventMapComponent } from './event-map.component';
import { KendoPocComponent } from './kendo-poc.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule,
        GridModule
    ],
    declarations: [
        EventAnalysisComponent,
        EventMapComponent,
        KendoPocComponent
    ],
    providers: [
        CrashService
    ]
})
export class EventAnalysisModule { }
