import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CrashQuery, CrashQueryRef, CrashResult, CrashService, EventAnalysisStateService } from './shared';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {

    geoExtent: 'Statewide' | 'County' | 'City' = 'Statewide';

    constructor(
        private route: ActivatedRoute,
        private modalService: NgbModal,
        private crashService: CrashService,
        private state: EventAnalysisStateService) { }

    get totalCount(): number {
        return this.state.crashQueryRef !== undefined
            ? this.state.crashQueryRef.totalCount
            : 0;
    }

    get mappedCount(): number {
        return this.state.crashQueryRef !== undefined
            ? this.state.crashQueryRef.mappedCount
            : 0;
    }

    get unmappedCount(): number {
        return this.state.crashQueryRef !== undefined
            ? this.state.crashQueryRef.unmappedCount
            : 0;
    }

    get dateTimeLabel(): string {
        let s = moment(this.state.startDate);
        let e = moment(this.state.endDate);
        if (s.year() === e.year()) {
            if (s.month() === e.month()) {
                // same year, same month
                return `${s.format('MMM D')} - ${e.format('D, YYYY')}`;
            }
            else {
                // same year, different month
                return `${s.format('MMM D')} - ${e.format('MMM D, YYYY')}`;
            }
        }
        else {
            // different year
            return `${s.format('MMM D, YYYY')} - ${e.format('MMM D, YYYY')}`;
        }
    }

    ngOnInit() {
        this.route.data
            .subscribe((data: { serverDate: Date }) => {
                this.state.endDate = data.serverDate;
                this.state.startDate = moment(data.serverDate).subtract(6, 'days').toDate();
                this.createCrashQuery();
            });
    }

    public openModal(content: any) {
        this.modalService.open(content).result.then((result) => {
            // closed
            this.createCrashQuery();
        }, (reason) => {
            // dismissed
        });
    }

    public pageChange(event: PageChangeEvent) {
        this.state.crashGridSkip = event.skip;
        this.loadCrashes();
    }

    private createCrashQuery() {
        this.state.crashQuery = {
            dateRange: {
                startDate: this.state.startDate,
                endDate: this.state.endDate
            }
        } as CrashQuery;
        this.crashService
            .createCrashQuery(this.state.crashQuery)
            .subscribe((queryRef: CrashQueryRef) => {
                this.state.crashQueryRef = queryRef;
                this.state.crashGridSkip = 0;
                this.loadCrashes();
            });
    }

    private loadCrashes() {
        let token = this.state.crashQueryRef.queryToken;
        let fromIndex = this.state.crashGridSkip;
        let toIndex = this.state.crashGridSkip + this.state.gridPageSize;
        let totalCount = this.state.crashQueryRef.totalCount;
        this.crashService
            .getCrashData(token, fromIndex, toIndex)
            .subscribe((results: CrashResult[]) => this.state.crashGridData = {
                data: results,
                total: totalCount
            });
    }
}
