import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import * as moment from 'moment';
import * as Highcharts from 'highcharts';
import { ReportOverTime } from './shared';

@Component({
    selector: 'data-timeliness',
    template: `<card>
        <ng-container card-header>
            <div class="font-weight-bold">{{header}}</div>
        </ng-container>
        <div class="m-3" card-block>
            <div id="dataTimeliness" class="mr-3"></div>
        </div>
        <ng-container card-footer>
            <div class="mt-2">
                Results shown through {{formattedMaxDate}}.
                <br /><i>Month and year shown reflect HSMV load date.</i>
            </div>
            <div>
                <button-group [items]="years" [(ngModel)]="reportYear"></button-group>
            </div>
        </ng-container>
    </card>`
})
export class DataTimelinessComponent implements OnInit, OnChanges {

    @Input() query: any;
    @Input() header: string;
    @Input() maxYear: Observable<number>;
    @Input() getEvents: (year: number, query: any) => Observable<ReportOverTime>;
    @Output() loaded = new EventEmitter<any>();

    years: number[];

    get reportYear(): number {
        return this._reportYear;
    }
    set reportYear(value: number) {
        this._reportYear = value;
        this.ngOnChanges();
    }

    private _reportYear: number;

    private sub: Subscription;
    private maxDate: moment.Moment;
    private chart: Highcharts.ChartObject;
    private initialized: boolean;

    private get formattedMaxDate(): string {
        return this.maxDate !== undefined ? this.maxDate.format('MMMM YYYY') : '';
    }

    ngOnInit() {
        let options: Highcharts.Options = {
            chart: {
                renderTo: 'dataTimeliness',
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
                pointFormat: '{series.name}: {point.y} ({point.percentage:.1f}%)'
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

        this.maxYear.subscribe((year: number) => {
            this.initialized = true;
            this.years = [year, year - 1, year - 2, year - 3];
            this.reportYear = year;
            this.retrieveData();
        });
    }

    ngOnChanges() {
        this.retrieveData();
    }

    private retrieveData() {
        if (!this.initialized) { return; }

        // cancel any prior request or the user may get unexpected results
        if (this.sub !== undefined && !this.sub.closed) {
            this.sub.unsubscribe();
        }

        if (this.chart !== undefined) {
            this.chart.showLoading();
        }

        this.sub = this
            .getEvents(this.reportYear, this.query)
            .subscribe(report => {
                this.maxDate = moment(report.maxDate);
                this.drawData(report);
            });
    }

    private drawData(report: ReportOverTime) {
        // set x-axis labels
        let options = { xAxis: { categories: report.categories } };
        this.chart.update(options);

        // remove old series
        while (this.chart.series.length > 0) {
            this.chart.series[0].remove(false);
        }

        // add new series
        for (let i = 0; i < report.series.length; i++) {
            let series = report.series[i];
            this.chart.addSeries(series, false);
        }

        // redraw and emit loaded event
        this.chart.redraw();
        this.chart.hideLoading();
        this.loaded.emit();
    }
}
