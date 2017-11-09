import { Component, OnChanges, Input } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highstock from 'highcharts/highstock';
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
                valueDecimals: 4,
                shared: true
            },
            plotOptions: {
                series: {
                    pointStart: Date.UTC(this.reportYear, 0, 1),
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
                    // xAxis: {
                    //     min: Date.UTC(this.reportYear, 0, 1),
                    //     max: Date.UTC(xAxisMaxDate.getFullYear(), xAxisMaxDate.getMonth(), xAxisMaxDate.getDate())
                    // },
                    series: [
                        /* {
                            // todo: implement flags server-side
                            // (irma flags are just for example)
                            type: 'flags',
                            data: this.yearOnYear ? [{
                                x: Date.UTC(2017, 8, 7),
                                title: '1',
                                text: 'Irma evacuation'
                            }, {
                                x: Date.UTC(2017, 8, 12),
                                title: '2',
                                text: 'Irma landfall'
                            }] : [],
                            onSeries: '2017',
                            shape: 'circlepin',
                            width: 16
                        }, */
                        ...(this.yearOnYear ? report.series : [report.series[1]])
                    ]
                };
                Highstock.stockChart('crashesByDay', options);
            });
    }
}
