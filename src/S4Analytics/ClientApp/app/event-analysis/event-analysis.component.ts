import { Component } from '@angular/core';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { CrashQuery, CrashQueryRef, CrashResult, CrashService } from './shared';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {
    public queryRef: CrashQueryRef;
    public gridView: GridDataResult;
    public pageSize = 10;
    public skip = 0;

    constructor(private crashService: CrashService) { }

    ngOnInit() {
        let query: CrashQuery = {
            dateRange: {
                startDate: new Date(2018, 0, 15),
                endDate: new Date(2018, 0, 15)
            }
        };
        this.crashService
            .createCrashQuery(query)
            .subscribe((queryRef: CrashQueryRef) => {
                this.queryRef = queryRef;
                this.loadCrashes();
            });
    }

    public pageChange(event: PageChangeEvent) {
        this.skip = event.skip;
        this.loadCrashes();
    }

    private loadCrashes() {
        this.crashService
            .getCrashData(this.queryRef.queryToken, this.skip, this.skip + this.pageSize)
            .subscribe((results: CrashResult[]) => this.gridView = {
                data: results,
                total: this.queryRef.totalCount
            });
    }
}
