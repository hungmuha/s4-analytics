import { Component, OnInit } from '@angular/core';
import { GridOptions } from 'ag-grid';
// import { CrashService, CrashQuery } from './shared';

@Component({
    selector: 'red-component',
    template: `<span style='color: red'>{{ params.value }}</span>`
})
export class RedComponent {
    private params: any;
    agInit(params: any): void {
        this.params = params;
    }
}

@Component({
    templateUrl: './ag-grid-poc.component.html'
})
export class AgGridPocComponent implements OnInit {
    private gridOptions: GridOptions;

    constructor() {
        this.gridOptions = {};
        this.gridOptions.columnDefs = [
            {
                headerName: 'ID',
                field: 'id',
                width: 100
            },
            {
                headerName: 'Value',
                field: 'value',
                cellRendererFramework: RedComponent,
                width: 100
            },

        ];
        this.gridOptions.rowData = [
            { id: 5, value: 10 },
            { id: 10, value: 15 },
            { id: 15, value: 20 }
        ];
    }
    ngOnInit() { }
}
