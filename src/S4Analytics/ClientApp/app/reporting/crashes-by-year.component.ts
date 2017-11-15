import { Component, OnChanges, Input } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highcharts from 'highcharts';
import { ReportingService, CrashesOverTimeQuery } from './shared';

@Component({
    selector: 'crashes-by-year',
    template: '<div id="crashesByYear"></div>'
})
export class CrashesByYearComponent implements OnChanges {

    @Input() query: CrashesOverTimeQuery;

    private sub: Subscription;

    constructor(private reporting: ReportingService) { }

    ngOnChanges() {
        this.refresh();
    }

    refresh() {
        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesByYear',
                type: 'column'
            },
            title: {
                text: 'Crashes over time by year'
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Total'
                },
                reversedStacks: false,
                stackLabels: {
                    enabled: true,
                    style: {
                        fontWeight: 'bold',
                        color: 'gray'
                    }
                }
            },
            legend: {
                align: 'right',
                x: -30,
                verticalAlign: 'top',
                y: 25,
                floating: true,
                backgroundColor: 'white',
                borderColor: '#CCC',
                borderWidth: 1,
                shadow: false
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
            },
            plotOptions: {
                column: {
                    stacking: 'normal',
                    dataLabels: {
                        enabled: true,
                        color: 'white'
                    }
                }
            }
        };

        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByYear(this.query)
            .subscribe(report => {
                options = { ...options, xAxis: { categories: report.categories }, series: report.series };
                Highcharts.chart(options);
            });
    }
}
