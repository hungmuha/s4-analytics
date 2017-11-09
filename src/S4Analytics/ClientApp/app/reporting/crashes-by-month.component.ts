import { Component, OnInit, OnChanges, Input } from '@angular/core';
import * as Highcharts from 'highcharts';
import { ReportingService } from './shared';

@Component({
    selector: 'crashes-by-month',
    template: '<div id="crashesByMonth"></div>'
})
export class CrashesByMonthComponent implements OnInit, OnChanges {

    @Input() reportYear: number;
    @Input() yearOnYear: boolean;

    constructor(private reporting: ReportingService) { }

    ngOnInit() {
        this.refresh();
    }

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
        this.reporting.getCrashesOverTimeByMonth(this.reportYear, this.yearOnYear).subscribe(report => {
            options = {
                ...options,
                xAxis: {
                    categories: report.categories
                },
                series: report.series
            };
            Highcharts.chart(options);
        });
    }
}
