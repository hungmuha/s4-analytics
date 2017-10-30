import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';
import * as Highstock from 'highcharts/highstock';
import { ReportingService } from './shared';

@Component({
    selector: 'crashes-over-time',
    templateUrl: './crashes-over-time.component.html'
})
export class CrashesOverTimeComponent implements OnInit {

    selected: 'year' | 'month' | 'day';

    constructor(private reporting: ReportingService) { }

    ngOnInit() {
        this.byYear();
    }

    byYear() {
        this.selected = 'year';
        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesOverTime',
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
        this.reporting.getCrashesOverTimeByYear().subscribe(report => {
            options = { ...options, xAxis: { categories: report.categories }, series: report.series };
            Highcharts.chart(options);
        });
    }

    byMonth() {
        this.selected = 'month';
        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesOverTime',
                type: 'line'
            },
            title: {
                text: 'Crashes over time by month, 2016-2017'
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
        this.reporting.getCrashesOverTimeByMonth().subscribe(report => {
            options = { ...options, xAxis: { categories: report.categories }, series: report.series };
            Highcharts.chart(options);
        });
    }

    byDay() {
        this.selected = 'day';

        let options: any = {
            chart: {
                renderTo: 'crashesOverTime'
            },
            rangeSelector: {
                selected: 4
            },
            yAxis: {
                plotLines: [{
                    value: 0,
                    width: 2,
                    color: 'silver'
                }]
            },
            plotOptions: {
                series: {
                    compare: 'percent',
                    showInNavigator: true
                }
            },
            tooltip: {
                pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.change}%)<br/>',
                valueDecimals: 2,
                split: true
            }
        };
        this.reporting.getCrashesOverTimeByDay().subscribe(report => {
            options = {
                ...options,
                // xAxis: { range: [{ min: new Date('01/01/2016'), max: new Date('12/31/2016') }, { min: new Date('01/01/2017'), max: new Date('12/31/2017') }] },
                series: report.series
            };
            Highstock.chart(options);
        });
    }
}
