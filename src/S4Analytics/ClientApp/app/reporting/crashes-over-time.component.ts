import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';
import { ReportingService } from './shared';

@Component({
    selector: 'crashes-over-time',
    templateUrl: './crashes-over-time.component.html'
})
export class CrashesOverTimeComponent implements OnInit {

    chart: Highcharts.ChartObject;

    constructor(private reporting: ReportingService) { }

    ngOnInit() {
        let options = {
            chart: {
                renderTo: 'crashesOverTime',
                type: 'column'
            },
            title: {
                text: 'Stacked column chart'
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
        this.reporting.getCrashesOverTime().subscribe(report => {
            options = { ...options, xAxis: { categories: report.categories }, series: report.series };
            this.chart = Highcharts.chart(options);
            // this.chart.options.xAxis = { categories: report.categories };
            // this.chart.options.series = report.series;
        });
    }
}
