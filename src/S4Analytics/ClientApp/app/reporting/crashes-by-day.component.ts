import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highstock from 'highcharts/highstock';
import * as moment from 'moment';
import {
    CrashReportingService,
    CrashesOverTimeQuery,
    ReportOverTime,
    EventsByDayFormatter
} from './shared';

@Component({
    selector: 'crashes-by-day',
    template: `<card>
        <ng-container card-header>
            <div class="font-weight-bold">Crashes by day</div>
        </ng-container>
        <div class="m-3" card-block>
            <div id="crashesByDay"></div>
        </div>
        <ng-container card-footer>
            <div class="mt-2">
                Results shown through {{formattedMaxDate}}.
            </div>
            <div>
                <label>
                    <input type="checkbox" [(ngModel)]="alignByWeek" [disabled]="!yearOnYear" />
                    Align by week
                </label>
                <label class="ml-2">
                    <input type="checkbox" [(ngModel)]="yearOnYear" />
                    Year-on-year
                </label>
                <button-group [items]="years" [(ngModel)]="reportYear"></button-group>
            </div>
        </ng-container>
    </card>`
})
export class CrashesByDayComponent implements OnInit, OnChanges {

    @Input() query: CrashesOverTimeQuery;
    @Output() loaded = new EventEmitter<any>();
    @Input() years: number[];

    get alignByWeek(): boolean {
        return this._alignByWeek;
    }
    set alignByWeek(value: boolean) {
        this._alignByWeek = value;
        this.ngOnChanges();
    }

    get yearOnYear(): boolean {
        return this._yearOnYear;
    }
    set yearOnYear(value: boolean) {
        this._yearOnYear = value;
        this.ngOnChanges();
    }

    get reportYear(): number {
        return this._reportYear;
    }
    set reportYear(value: number) {
        this._reportYear = value;
        this.ngOnChanges();
    }

    private _alignByWeek: boolean;
    private _yearOnYear: boolean;
    private _reportYear: number;

    private sub: Subscription;
    private maxDate: moment.Moment;
    private chart: Highstock.ChartObject;
    private initialized: boolean;

    private get formattedMaxDate(): string {
        return this.maxDate !== undefined ? this.maxDate.format('MMMM DD, YYYY') : '';
    }

    constructor(private reporting: CrashReportingService) { }

    ngOnInit() {
        this.alignByWeek = true;
        this.yearOnYear = true;
        this.reportYear = this.years[0];

        let options: Highstock.Options = {
            title: {
                text: ''
            },
            rangeSelector: {
                buttons: [{
                    type: 'month',
                    count: 1,
                    text: '1m'
                }, {
                    type: 'month',
                    count: 3,
                    text: '3m'
                }, {
                    type: 'month',
                    count: 6,
                    text: '6m'
                }, {
                    type: 'all',
                    text: '1y'
                }]
            },
            tooltip: {
                valueDecimals: 0,
                shared: true,
                crosshairs: true
            },
        };
        this.chart = Highstock.stockChart('crashesByDay', options);

        this.initialized = true;
        this.retrieveData();
    }

    ngOnChanges() {
        this.retrieveData();
    }

    private retrieveData() {
        if (!this.initialized) { return; }

        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByDay(this.reportYear, this.alignByWeek, this.query)
            .subscribe(report => {
                this.maxDate = moment(report.maxDate);
                this.drawData(report);
            });
    }

    private drawData(report: ReportOverTime) {
        // set x-axis labels
        let options = {
            xAxis: { categories: report.categories },
            plotOptions: {
                series: {
                    pointStart: this.xAxisStartUtc(),
                    pointInterval: 24 * 3600 * 1000 // one day
                }
            },
            tooltip: {
                formatter: this.alignByWeek
                    ? (new EventsByDayFormatter()).formatAlignedByWeek // do not use arrow function because highcharts provides the context for the format method
                    : (new EventsByDayFormatter()).formatAlignedByDate
            }
        };
        this.chart.update(options);

        // remove old series
        while (this.chart.series.length > 0) {
            this.chart.series[0].remove(false);
        }

        // add new series
        for (let i = 0; i < report.series.length; i++) {
            if (this.yearOnYear || i === 1) { // i=0: prior year; i=1: selected year
                let series = report.series[i];
                this.chart.addSeries(series, false);
            }
        }

        // redraw and emit loaded event
        this.chart.redraw();
        this.loaded.emit();
    }

    private xAxisStartUtc(): number {
        // the x axis represents the current year.
        // if the prior year series is aligned by day of week,
        // there will be 1 or 2 data points before jan 1 of
        // the current year series. we have to set the start
        // date of the x axis accordingly.
        let startDate = moment([this.reportYear, 0, 1]);
        if (this.yearOnYear && this.alignByWeek) {
            let isPriorYearLeapYear = moment([this.reportYear - 1]).isLeapYear();
            let offsetDays = isPriorYearLeapYear ? 2 : 1; // 366 % 7 = 2 whereas 365 % 7 = 1
            startDate = startDate.subtract(offsetDays, 'days');
        }
        let startUtc = Date.UTC(startDate.year(), startDate.month(), startDate.date());
        return startUtc;
    }
}
