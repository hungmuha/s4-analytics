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
        this.byDay();
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
            rangeSelector: {
                selected: 4
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
                pointFormat: '<span style="color:{series.color}">{series.name}</span>: {Highcharts.dateFormat(" % A, %b % e, %Y", this.x)} <b>{point.y}</b><br/>',
                valueDecimals: 2,
                split: true
            },
            plotOptions: {
                series: {
                    pointStart: Date.UTC(2017, 0, 1),
                    pointInterval: 24 * 3600 * 1000 // one day
                }
            }
        };
        this.reporting.getCrashesOverTimeByDay().subscribe(report => {
            options = {
                ...options,
                series: report.series
            };
            Highstock.stockChart('crashesOverTime', options);
        });
    }
}
