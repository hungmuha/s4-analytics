import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import * as _ from 'lodash';
import { CitationsOverTimeQuery, CitationReportingService } from './shared';

class Lookup {
    key: number;
    name: string;
}

@Component({
    selector: 'citations-over-time',
    templateUrl: './citations-over-time.component.html'
})
export class CitationsOverTimeComponent implements OnInit {

    geographies: Lookup[];
    agencies: Lookup[];
    years: number[];
    yesNo = ['Yes', 'No'];
    violationClassification = ['Any', 'Bicyclist', 'Criminal', 'Moving', 'Non-Moving', 'Pedestrian', 'Unknown'];

    selectedGeography: Lookup | string;
    selectedAgency: Lookup | string;

    query = new CitationsOverTimeQuery();
    citationsByYearLoaded: boolean;
    citationsByMonthLoaded: boolean;
    citationsByDayLoaded: boolean;
    citationsByAttributeLoaded: boolean;
    selectedCrashInvolved?: string = undefined;
    selectedClassification?: string = 'Any';

    reportAttributes: { [key: string]: string } = {
        'violation-type': 'Violation Type',
        'violator-age': 'Violator Age',
        'violator-gender': 'Violator Gender'
    };

    get loading(): boolean {
        return !(this.citationsByYearLoaded && this.citationsByMonthLoaded && this.citationsByDayLoaded && this.citationsByAttributeLoaded);
    }

    constructor(private reporting: CitationReportingService) { }

    getCitationsByYear =
        (query: CitationsOverTimeQuery) => this.reporting.getCitationsOverTimeByYear(query)
    getCitationsByMonth =
        (year: number, query: CitationsOverTimeQuery) => this.reporting.getCitationsOverTimeByMonth(year, query)
    getCitationsByDay =
        (year: number, alignByWeek: boolean, query: CitationsOverTimeQuery) => this.reporting.getCitationsOverTimeByDay(year, alignByWeek, query)
    getCitationsByAttribute =
        (year: number, attrName: string, query: CitationsOverTimeQuery) => this.reporting.getCitationsOverTimeByAttribute(year, attrName, query)

    ngOnInit() {
        this.beginLoad();
        this.reporting.getReportingAgencies().subscribe(results => this.agencies = results);
        this.reporting.getGeographies().subscribe(results => this.geographies = results);
        let currentYear = (new Date()).getFullYear();
        this.years = [currentYear, currentYear - 1, currentYear - 2, currentYear - 3];
    }

    beginLoad() {
        this.citationsByYearLoaded = this.citationsByMonthLoaded = this.citationsByDayLoaded = this.citationsByAttributeLoaded = false;
    }

    formatLookup(value: Lookup) {
        return value.name;
    }

    searchGeographies = (text: Observable<string>) =>
        text.debounceTime(200)
            .distinctUntilChanged()
            .map(term => term.length === 0
                ? []
                : _.filter(this.geographies, g => g.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))

    searchAgencies = (text: Observable<string>) =>
        text.debounceTime(200)
            .distinctUntilChanged()
            .map(term => term.length === 0
                ? []
                : _.filter(this.agencies, g => g.name.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))

    refresh() {
        this.beginLoad();

        // clear geography & agency fields if a valid selection was not made
        if (this.selectedGeography === undefined || !this.selectedGeography.hasOwnProperty('key')) {
            this.selectedGeography = '';
        }
        if (this.selectedAgency === undefined || !this.selectedAgency.hasOwnProperty('key')) {
            this.selectedAgency = '';
        }

        let query: CitationsOverTimeQuery = {
            geographyId: this.selectedGeography !== ''
                ? (this.selectedGeography as Lookup).key
                : undefined,
            reportingAgencyId: this.selectedAgency !== ''
                ? (this.selectedAgency as Lookup).key
                : undefined,
            crashInvolved: this.selectedCrashInvolved == undefined
                ? undefined
                : this.selectedCrashInvolved === 'Yes',
            classification: this.selectedClassification !== undefined
                ? this.selectedClassification
                : undefined
        };
        this.query = query;
    }
}