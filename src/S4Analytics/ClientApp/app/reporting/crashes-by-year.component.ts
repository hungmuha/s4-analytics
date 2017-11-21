import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highcharts from 'highcharts';
import { CrashReportingService, CrashesOverTimeQuery, ReportOverTime } from './shared';

@Component({
    selector: 'crashes-by-year',
    template: `<card>
        <ng-container card-header>
            <div class="font-weight-bold">Crashes by year</div>
        </ng-container>
        <div class="m-3" card-block>
            <div id="crashesByYear" class="mr-3"></div>
        </div>
        <ng-container card-footer>
            Results shown for past 5 years only.
        </ng-container>
    </card>`
})
export class CrashesByYearComponent implements OnInit, OnChanges {

    @Input() query: CrashesOverTimeQuery;
    @Output() loaded = new EventEmitter<any>();

    private sub: Subscription;
    private chart: Highcharts.ChartObject;

    constructor(private reporting: CrashReportingService) { }

    ngOnInit() {
        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesByYear',
                type: 'column'
            },
            title: {
                text: ''
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
            },
            lang: {
                loading: '',
            },
            series: []
        };
        this.chart = Highcharts.chart(options);
        this.chart.showLoading();
    }

    ngOnChanges() {
        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        if (this.chart !== undefined) {
            this.chart.showLoading();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByYear(this.query)
            .subscribe(report => this.drawReportData(report));
    }

    private drawReportData(report: ReportOverTime) {
        // set x-axis labels
        let options = { xAxis: { categories: report.categories } };
        this.chart.update(options);

        // remove old series
        while (this.chart.series.length > 0) {
            this.chart.series[0].remove(false);
        }

        // add new series
        for (let series of report.series) {
            this.chart.addSeries(series, false);
        }

        // redraw and emit loaded event
        this.chart.redraw();
        this.chart.hideLoading();
        this.loaded.emit();
    }
}
