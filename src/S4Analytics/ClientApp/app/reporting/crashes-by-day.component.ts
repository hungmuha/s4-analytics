import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highstock from 'highcharts/highstock';
import * as moment from 'moment';
import { ReportingService, CrashesOverTimeQuery, ReportOverTime } from './shared';

@Component({
    selector: 'crashes-by-day',
    template: '<div id="crashesByDay"></div>'
})
export class CrashesByDayComponent implements OnInit, OnChanges {

    @Input() reportYear: number;
    @Input() yearOnYear: boolean;
    @Input() alignByWeek: boolean;
    @Input() query: CrashesOverTimeQuery;
    @Output() loaded = new EventEmitter<any>();

    private sub: Subscription;
    private chart: Highstock.ChartObject;

    constructor(private reporting: ReportingService) { }

    ngOnInit() {
        let options: any = {
            title: {
                text: 'Crashes over time by day'
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
            yAxis: {
                top: '10%',
                height: '90%',
                plotLines: [{
                    value: 0,
                    width: 2,
                    color: 'silver'
                }]
            },
            tooltip: {
                style: {
                    width: '200px'
                },
                valueDecimals: 0,
                shared: true
            }
        };
        this.chart = Highstock.stockChart('crashesByDay', options);
    }

    ngOnChanges() {
        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByDay(this.reportYear, this.alignByWeek, this.query)
            .subscribe(report => this.drawReportData(report));
    }

    private drawReportData(report: ReportOverTime) {
        // set x-axis labels
        let options = {
            xAxis: { categories: report.categories },
            plotOptions: {
                series: {
                    pointStart: this.xAxisStartUtc(),
                    pointInterval: 24 * 3600 * 1000 // one day
                }
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
