import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highcharts from 'highcharts';
import { ReportingService, CrashesOverTimeQuery, ReportOverTime } from './shared';

@Component({
    selector: 'crashes-by-month',
    template: '<div id="crashesByMonth"></div>'
})
export class CrashesByMonthComponent implements OnInit, OnChanges {

    @Input() reportYear: number;
    @Input() yearOnYear: boolean;
    @Input() query: CrashesOverTimeQuery;
    @Output() loaded = new EventEmitter<any>();

    private sub: Subscription;
    private chart: Highcharts.ChartObject;

    constructor(private reporting: ReportingService) { }

    ngOnInit() {
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
        this.chart = Highcharts.chart(options);
    }

    ngOnChanges() {
        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        this.sub = this.reporting
            .getCrashesOverTimeByMonth(this.reportYear, this.query)
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
        for (let i = 0; i < report.series.length; i++) {
            if (this.yearOnYear || i === 0) {
                let series = report.series[i];
                this.chart.addSeries(series, false);
            }
        }

        // redraw and emit loaded event
        this.chart.redraw();
        this.loaded.emit();
    }
}
