import { Component, OnInit, OnChanges, Input, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import * as Highcharts from 'highcharts';
import * as moment from 'moment';
import { CrashReportingService, CrashesOverTimeQuery, ReportOverTime } from './shared';

let total = 0;

class EventsByAttributeFormatter {
    /*  Note that you will never have an actual instance of this class. Instead the format methods
        below will be bound to a context object provided by Highcharts. The context object will have
        more fields than what are defined below, but we only need to define the ones we use.
        See https://api.highcharts.com/highcharts/tooltip.formatter for info on available context data. */

    x: number;
    y: number;

    formatter(): string {
        let perc = (this.y / total) * 100;
        let percFormatted = perc < 1 ? '< 1%' : perc.toFixed(1) + '%';
        return `<b>${this.x}</b><br/>${this.y} (${percFormatted})`;
    }
}

@Component({
    selector: 'crashes-by-attribute',
    template: `<card>
        <ng-container card-header>
            <div class="font-weight-bold">Crashes by attribute</div>
        </ng-container>
        <div class="m-3" card-block>
            <div id="crashesByAttribute" class="mr-3"></div>
        </div>
        <ng-container card-footer>
            <div class="mt-2">
                Results shown through {{formattedMaxDate}}.
            </div>
            <div>
                <select class="custom-select" (change)="selectedAttributeKey=$event.target.value">
                    <option *ngFor="let attrKey of attributeKeys"
                            [value]="attrKey"
                            [selected]="selectedAttributeKey===attrKey">
                        {{attributes[attrKey]}}
                    </option>
                </select>
                <button-group [items]="years" [(ngModel)]="reportYear"></button-group>
            </div>
        </ng-container>
    </card>`
})
export class CrashesByAttributeComponent implements OnInit, OnChanges {

    @Input() query: CrashesOverTimeQuery;
    @Input() years: number[];
    @Output() loaded = new EventEmitter<any>();

    get reportYear(): number {
        return this._reportYear;
    }
    set reportYear(value: number) {
        this._reportYear = value;
        this.ngOnChanges();
    }

    attributes: { [key: string]: string } = {
        'hour-of-day': 'Hour of day',
        'day-of-week': 'Day of week',
        'crash-type': 'Crash type',
        'crash-severity': 'Crash severity',
        'light-condition': 'Light condition',
        'road-surface-condition': 'Road surface condition',
        'weather-condition': 'Weather condition',
        'first-harmful-event': 'First harmful event'
    };

    get attributeKeys(): string[] {
        return Object.keys(this.attributes);
    }

    get selectedAttributeKey(): string {
        return this._selectedAttribute;
    }
    set selectedAttributeKey(value: string) {
        this._selectedAttribute = value;
        this.ngOnChanges();
    }
    private _selectedAttribute: string;

    private _reportYear: number;

    private sub: Subscription;
    private maxDate: moment.Moment;
    private chart: Highcharts.ChartObject;

    private get formattedMaxDate(): string {
        return this.maxDate !== undefined ? this.maxDate.format('MMMM DD, YYYY') : '';
    }

    constructor(private reporting: CrashReportingService) { }

    ngOnInit() {
        this.reportYear = this.years[0];
        this.selectedAttributeKey = this.attributeKeys[0];

        let options: Highcharts.Options = {
            chart: {
                renderTo: 'crashesByAttribute',
                type: 'column'
            },
            title: {
                text: ''
            },
            yAxis: {
                min: 0
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
                formatter: (new EventsByAttributeFormatter()).formatter
            },
            plotOptions: {
                column: {
                    dataLabels: {
                        enabled: true,
                        color: 'gray'
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
            .getCrashesOverTimeByAttribute(this.reportYear, this.selectedAttributeKey, this.query)
            .subscribe(report => {
                this.maxDate = moment(report.maxDate);
                this.drawReportData(report);
            });
    }

    private drawReportData(report: ReportOverTime) {
        let attrName = this.attributes[this.selectedAttributeKey];

        let series = {
            id: attrName,
            name: attrName,
            data: report.series[0].data
        };

        // get total for calculating percentages in tooltips
        total = 0;
        for (let value of series.data) {
            total += value;
        }

        // set x-axis labels
        let options = { xAxis: { categories: report.categories } };
        this.chart.update(options);

        // remove old series
        while (this.chart.series.length > 0) {
            this.chart.series[0].remove(false);
        }

        // add new series
        this.chart.addSeries(series, false);

        // redraw and emit loaded event
        this.chart.redraw();
        this.chart.hideLoading();
        this.loaded.emit();
    }
}
