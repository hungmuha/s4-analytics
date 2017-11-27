import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import * as _ from 'lodash';
import { CitationsOverTimeQuery, CitationReportingService } from './shared';


@Component({
    selector: 'citations-over-time',
    templateUrl: './citations-over-time.component.html'
})
export class CitationsOverTimeComponent implements OnInit {

    years: number[];
    query = new CitationsOverTimeQuery();

    constructor(private reporting: CitationReportingService) { }

    ngOnInit() {
        let currentYear = (new Date()).getFullYear();
        this.years = [currentYear, currentYear - 1, currentYear - 2, currentYear - 3];
    }


}