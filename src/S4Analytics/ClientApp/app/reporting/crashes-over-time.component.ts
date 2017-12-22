import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/debounceTime';
import 'rxjs/add/operator/distinctUntilChanged';
import * as _ from 'lodash';
import { CrashesOverTimeQuery, CrashReportingService } from './shared';

class Lookup {
    key: number;
    name: string;
}

@Component({
    selector: 'crashes-over-time',
    templateUrl: './crashes-over-time.component.html'
})
export class CrashesOverTimeComponent implements OnInit {

    geographies: Lookup[];
    agencies: Lookup[];
    severities = ['Fatal', 'Injury', 'PDO'];
    impairments = ['Alcohol', 'Drugs'];
    bikePedTypes = ['Bike', 'Ped'];
    yesNo = ['Yes', 'No'];
    formTypes = ['Long', 'Short'];
    years: number[];

    selectedGeography: Lookup | string;
    selectedAgency: Lookup | string;
    selectedSeverities: string[] = [];
    selectedImpairments: string[] = [];
    selectedBikePedTypes: string[] = [];
    selectedCmvRelated?: string = undefined;
    selectedCodeable?: string = undefined;
    selectedFormType?: string = undefined;

    query = new CrashesOverTimeQuery();
    crashesByYearLoaded: boolean;
    crashesByMonthLoaded: boolean;
    crashesByDayLoaded: boolean;
    crashesByAttributeLoaded: boolean;
    // dataTimelinessLoaded: boolean;

    reportAttributes: { [key: string]: string } = {
        'hour-of-day': 'Hour of day',
        'day-of-week': 'Day of week',
        'crash-type': 'Crash type',
        'crash-severity': 'Crash severity',
        'light-condition': 'Light condition',
        'road-surface-condition': 'Road surface condition',
        'weather-condition': 'Weather condition',
        'first-harmful-event': 'First harmful event'
    };

    get loading(): boolean {
        let allLoaded =
            this.crashesByYearLoaded &&
            this.crashesByMonthLoaded &&
            this.crashesByDayLoaded &&
            this.crashesByAttributeLoaded; // &&
            // this.dataTimelinessLoaded;
        return !allLoaded;
    }

    constructor(private reporting: CrashReportingService) { }

    getCrashesByYear =
        (query: CrashesOverTimeQuery) => this.reporting.getCrashesOverTimeByYear(query)
    getCrashesByMonth =
        (year: number, query: CrashesOverTimeQuery) => this.reporting.getCrashesOverTimeByMonth(year, query)
    getCrashesByDay =
        (year: number, alignByWeek: boolean, query: CrashesOverTimeQuery) => this.reporting.getCrashesOverTimeByDay(year, alignByWeek, query)
    getCrashesByAttribute =
        (year: number, attrName: string, query: CrashesOverTimeQuery) => this.reporting.getCrashesOverTimeByAttribute(year, attrName, query)
    getDataTimeliness =
        (year: number, query: CrashesOverTimeQuery) => this.reporting.getDataTimeliness(year, query)

    ngOnInit() {
        this.beginLoad();
        this.reporting.getReportingAgencies().subscribe(results => this.agencies = results);
        this.reporting.getGeographies().subscribe(results => this.geographies = results);
        let currentYear = (new Date()).getFullYear();
        this.years = [currentYear, currentYear - 1, currentYear - 2, currentYear - 3];
    }

    beginLoad() {
        this.crashesByYearLoaded =
            this.crashesByMonthLoaded =
            this.crashesByDayLoaded =
            this.crashesByAttributeLoaded = false;
            // this.dataTimelinessLoaded = false;
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

        let query: CrashesOverTimeQuery = {
            geographyId: this.selectedGeography !== ''
                ? (this.selectedGeography as Lookup).key
                : undefined,
            reportingAgencyId: this.selectedAgency !== ''
                ? (this.selectedAgency as Lookup).key
                : undefined,
            severity: this.selectedSeverities.length > 0
                ? {
                    fatality: _.includes(this.selectedSeverities, 'Fatal'),
                    injury: _.includes(this.selectedSeverities, 'Injury'),
                    propertyDamageOnly: _.includes(this.selectedSeverities, 'PDO')
                } : undefined,
            impairment: this.selectedImpairments.length > 0
                ? {
                    alcoholRelated: _.includes(this.selectedImpairments, 'Alcohol'),
                    drugRelated: _.includes(this.selectedImpairments, 'Drugs')
                } : undefined,
            bikePedRelated: this.selectedBikePedTypes.length > 0
                ? {
                    bikeRelated: _.includes(this.selectedBikePedTypes, 'Bike'),
                    pedRelated: _.includes(this.selectedBikePedTypes, 'Ped')
                } : undefined,
            cmvRelated: this.selectedCmvRelated !== undefined
                ? this.selectedCmvRelated === 'Yes'
                : undefined,
            codeable: this.selectedCodeable !== undefined
                ? this.selectedCodeable === 'Yes'
                : undefined,
            formType: this.selectedFormType !== undefined
                ? {
                    longForm: this.selectedFormType === 'Long',
                    shortForm: this.selectedFormType === 'Short'
                } : undefined
        };
        this.query = query;
    }
}
