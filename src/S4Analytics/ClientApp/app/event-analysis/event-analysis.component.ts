import { Component, OnInit } from '@angular/core';
import { CrashService, CrashQuery } from './shared';
import { GridDataResult, SelectionEvent, PageChangeEvent } from '@progress/kendo-angular-grid';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent implements OnInit {
    gridData: GridDataResult;
    sort: SortDescriptor[] = [];
    query: CrashQuery;
    pageSize: number = 100;
    skip: number = 0;

    constructor(private crashService: CrashService) { }

    ngOnInit() {
        this.query = {
            dateRange: {
                startDate: new Date('2017-04-01'),
                endDate: new Date('2017-04-07')
            }
        };
        this.loadData();
    }

    sortChange(sort: SortDescriptor[]): void {
        this.sort = sort;
        this.loadData();
    }

    selectionChange(event: SelectionEvent) {
        console.log(event);
    }

    pageChange(event: PageChangeEvent) {
        console.log(event);
        if (!isNaN(event.skip)) {
            this.skip = event.skip;
            this.loadData();
        }
    }

    loadData() {
        this.crashService.getCrashData(this.query, this.skip, this.skip + this.pageSize)
            .subscribe((data: any) => {
                this.gridData = {
                    data: orderBy(data, this.sort),
                    total: 1000 // todo: get the actual total from the API somehow
                };
            });
    }
}
