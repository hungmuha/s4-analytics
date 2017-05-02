import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GridModule } from '@progress/kendo-angular-grid';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { AgGridModule } from 'ag-grid-angular/main';
import { CrashService } from './shared';
import { EventAnalysisComponent } from './event-analysis.component';
import { EventMapComponent } from './event-map.component';
import { KendoPocComponent } from './kendo-poc.component';
import { NgxDatatablePocComponent } from './ngx-datatable-poc.component';
import { AgGridPocComponent, RedComponent } from './ag-grid-poc.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule,
        GridModule,
        NgxDatatableModule,
        AgGridModule.withComponents(
            [RedComponent]
        )
    ],
    declarations: [
        EventAnalysisComponent,
        EventMapComponent,
        KendoPocComponent,
        NgxDatatablePocComponent,
        AgGridPocComponent,
        RedComponent
    ],
    providers: [
        CrashService
    ]
})
export class EventAnalysisModule { }
