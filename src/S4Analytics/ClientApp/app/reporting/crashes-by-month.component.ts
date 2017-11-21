import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as moment from 'moment';
import * as Highcharts from 'highcharts';
import { CrashReportingService, CrashesOverTimeQuery, ReportOverTime } from './shared';

@Component({
    selector: 'crashes-by-month',
    template: `<card>
        <ng-container card-header>
            <div class="font-weight-bold">Crashes by month</div>
        </ng-container>
        <div class="m-3" card-block>
            <div id="crashesByMonth" class="mr-3"></div>
        </div>
        <ng-container card-footer>
            <div class="mt-2">
                Results shown through {{formattedMaxDate}}.
            </div>
            <div>
                <label>
                    <input type="checkbox" [(ngModel)]="yearOnYear" />
                    Year-on-year
                </label>
                <button-group [items]="years" [(ngModel)]="reportYear"></button-group>
            </div>
        </ng-container>
    </card>`
})
export class CrashesByMonthComponent implements OnInit, OnChanges {

    @Input() query: CrashesOverTimeQuery;
    @Output() loaded = new EventEmitter<any>();
    @Input() years: number[];

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

    private _yearOnYear: boolean;
    private _reportYear: number;

    private sub: Subscription;
    private maxDate: moment.Moment;
    private chart: Highcharts.ChartObject;
    private initialized: boolean;

    private get formattedMaxDate(): string {
        return this.maxDate !== undefined ? this.maxDate.format('MMMM YYYY') : '';
    }

    constructor(private reporting: CrashReportingService) { }

    ngOnInit() {
        this.yearOnYear = true;
        this.reportYear = this.years[0];

        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesByMonth',
                type: 'line'
            },
            title: {
                text: ''
            },
            yAxis: {
                title: {
                    text: 'Total'
                }
            },
            tooltip: {
                shared: true,
                crosshairs: true
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: true
                }
            },
            lang: {
                loading: '',
            }
        };
        this.chart = Highcharts.chart(options);
        this.chart.showLoading();
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

        if (this.chart !== undefined) {
            this.chart.showLoading();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByMonth(this.reportYear, this.query)
            .subscribe(report => {
                this.maxDate = moment(report.maxDate);
                this.drawData(report);
            });
    }

    private drawData(report: ReportOverTime) {
        // set x-axis labels
        let options = { xAxis: { categories: report.categories } };
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
        this.chart.hideLoading();
        this.loaded.emit();
    }
}
