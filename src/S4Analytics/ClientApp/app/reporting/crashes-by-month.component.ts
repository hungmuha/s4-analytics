import { Component, OnChanges, Input } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highcharts from 'highcharts';
import { ReportingService } from './shared';

@Component({
    selector: 'crashes-by-month',
    template: '<div id="crashesByMonth"></div>'
})
export class CrashesByMonthComponent implements OnChanges {

    private sub: Subscription;

    @Input() reportYear: number;
    @Input() yearOnYear: boolean;

    constructor(private reporting: ReportingService) { }

    ngOnChanges() {
        this.refresh();
    }

    refresh() {
        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesByMonth',
                type: 'line'
            },
            title: {
                text: 'Crashes over time by month'
            },
            yAxis: {
                title: {
                    text: 'Total'
                }
            },
            plotOptions: {
                line: {
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false
                }
            }
        };

        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByMonth(this.reportYear)
            .subscribe(report => {
                options = {
                    ...options,
                    xAxis: {
                        categories: report.categories
                    },
                    series: this.yearOnYear ? report.series : [ report.series[1] ]
                };
                Highcharts.chart(options);
            });
    }
}
