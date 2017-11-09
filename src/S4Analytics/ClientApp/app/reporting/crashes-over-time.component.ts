import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'crashes-over-time',
    templateUrl: './crashes-over-time.component.html'
})
export class CrashesOverTimeComponent implements OnInit {

    private currentYear = (new Date()).getFullYear();

    selected: 'year' | 'month' | 'day';
    reportYear: number;
    yearOnYear: boolean;
    alignByWeek: boolean;

    ngOnInit() {
        this.reportYear = this.currentYear;
        this.yearOnYear = true;
        this.alignByWeek = true;
        this.byYear();
    }

    get years(): number[] {
        let vals = [this.currentYear, this.currentYear-1, this.currentYear-2, this.currentYear-3];
        return vals;
    }

    byYear() {
        this.selected = 'year';
    }

    byMonth() {
        this.selected = 'month';
    }

    byDay() {
        this.selected = 'day';
    }
}
