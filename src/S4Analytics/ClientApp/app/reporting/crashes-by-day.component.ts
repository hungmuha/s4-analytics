import { Component, OnChanges, Input } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highstock from 'highcharts/highstock';
import * as moment from 'moment';
import { ReportingService } from './shared';

@Component({
    selector: 'crashes-by-day',
    template: '<div id="crashesByDay"></div>'
})
export class CrashesByDayComponent implements OnChanges {

    @Input() reportYear: number;
    @Input() yearOnYear: boolean;
    @Input() alignByWeek: boolean;

    private sub: Subscription;

    constructor(private reporting: ReportingService) { }

    ngOnChanges() {
        this.refresh();
    }

    refresh() {
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
            },
            plotOptions: {
                series: {
                    pointStart: this.xAxisStartUtc(),
                    pointInterval: 24 * 3600 * 1000 // one day
                }
            }
        };

        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByDay(this.reportYear, this.alignByWeek)
            .subscribe(report => {
                let xAxisMaxDate = new Date(report.maxDate);
                xAxisMaxDate.setDate(xAxisMaxDate.getDate() + 5);
                // configure and create chart
                options = {
                    ...options,
                    series: this.yearOnYear ? report.series : [report.series[1]]
                };
                Highstock.stockChart('crashesByDay', options);
            });
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
