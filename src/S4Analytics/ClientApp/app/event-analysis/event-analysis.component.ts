import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import * as _ from 'lodash';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { LookupService, LookupKeyAndName } from '../shared';
import { DateTimeScope, PlaceScope, QueryRef, CrashResult, CrashService, EventAnalysisStateService } from './shared';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {
    rdSysIds: LookupKeyAndName[];
    crashTypes: LookupKeyAndName[];
    crashSeverity: LookupKeyAndName[];
    weatherCondition: LookupKeyAndName[];
    cmvInvolved: [{ key: string, name: string }];
    dayOrNight: [{ key: string, name: string }];
    intersectionRelated: [{ key: string, name: string }];
    laneDeparture: [{ key: string, name: string }];
    formType: [{ key: string, name: string }];
    codeable: [{ key: string, name: string }];
    bikePedInvolved: [{ key: string, name: string }];
    // crashes:LookupKeyAndName[]
    // behavioralFactors: LookupKeyAndName[];
    // reportingAgency: LookupKeyAndName[];

    constructor(
        private route: ActivatedRoute,
        private crashService: CrashService,
        private state: EventAnalysisStateService,
        private lookup: LookupService) { }

    get totalCount(): number {
        return this.state.crashQueryRef !== undefined
            ? this.state.crashQueryRef.totalCount
            : 0;
    }

    get mappedCount(): number {
        return this.state.crashQueryRef !== undefined
            ? this.state.crashQueryRef.mappedCount
            : 0;
    }

    get unmappedCount(): number {
        return this.state.crashQueryRef !== undefined
            ? this.state.crashQueryRef.unmappedCount
            : 0;
    }

    applyDateTimeScope(value: DateTimeScope) {
        this.state.dateTimeScope = value;
        this.issueCrashQuery();
    }

    applyPlaceScope(value: PlaceScope) {
        this.state.placeScope = value;
        this.issueCrashQuery();
    }

    get dateTimeLabel(): string {
        /*
        Examples:
        (same month)                 Jan 1 - 7, 2018
        (same year, different month) Jan 1 - Feb 28, 2018
        (different year)             Dec 1, 2017 - Jan 31, 2018
        */

        let s = moment(this.state.dateTimeScope.dateRange.startDate);
        let e = moment(this.state.dateTimeScope.dateRange.endDate);
        let sameYear = s.year() === e.year();
        let sameMonth = sameYear && s.month() === e.month();
        let label: string;

        if (sameMonth) {
            label = `${s.format('MMM D')} - ${e.format('D, YYYY')}`;
        }
        else if (sameYear) {
            label = `${s.format('MMM D')} - ${e.format('MMM D, YYYY')}`;
        }
        else {
            label = `${s.format('MMM D, YYYY')} - ${e.format('MMM D, YYYY')}`;
        }
        return label;
    }

    get placeLabel(): string {
        /*
        Examples:
        (statewide)    Statewide
        (<=3 counties) County: Alachua, Marion
        (> 3 cities)   City: Gainesville, Alachua, High Springs, +2
        */

        let label: string;
        let items: string[] = [];

        if (this.state.placeScope.county !== undefined && this.state.placeScope.county.length > 0) {
            label = 'County';
            items = _.filter(this.state.allCounties, kn => _.includes(this.state.placeScope.county, kn.key)).map(kn => kn.name);
        }
        else if (this.state.placeScope.city !== undefined && this.state.placeScope.city.length > 0) {
            label = 'City';
            items = _.filter(this.state.allCities, kn => _.includes(this.state.placeScope.city, kn.key)).map(kn => kn.name);
        }
        else {
            label = 'Statewide';
        }

        if (items.length > 0) {
            label = `${label}: ${items.slice(0, 3).join(', ')}`;
            if (items.length > 3) {
                label = `${label}, +${items.length - 3}`;
            }
        }

        return label;
    }

    set codeableOnly(results: string[]) {
        if (results && results.length > 0) {
            this.state.crashQuery.codeableOnly = results[0] === 'Y' ? true : false;
        }
    }

    set isCmvInvolved(results: string[]) {
        if (results && results.length > 0) {
            this.state.crashQuery.cmvInvolved = results[0] === 'Y' ? true : false;
        }
    }

    set isIntersectionRelated(results: string[]) {
        if (results && results.length > 0) {
            this.state.crashQuery.intersectionRelated = results[0] === 'Y' ? true : false;
        }
    }

    set isBikePedInvolved(results: string[]) {
        if (results && results.length > 0) {
            this.state.crashQuery.bikePedInvolved = {
                bikeInvolved: results.indexOf('Bike') >= 0 ? true : false,
                pedInvolved: results.indexOf('Ped') >= 0 ? true : false
            };
        }
    }

    ngOnInit() {
        this.route.data
            .subscribe((data: { serverDate: Date }) => {
                // set initial date range; issue query
                this.setInitialDateRange(data.serverDate);
                this.setInitialPlace();
                this.issueCrashQuery();
            });
        this.initFilters();
    }

    initFilters() {
        this.lookup.getCounties().subscribe(results => {
            this.state.allCounties = results;
        });
        this.lookup.getCities().subscribe(results => {
            this.state.allCities = results;
        });
        this.lookup.getRoadSystemIdentifiers().subscribe(results => {
            this.rdSysIds = results;
        });
        this.lookup.getCrashTypes().subscribe(results => {
            this.crashTypes = results;
        });
        this.lookup.getCrashSeverity().subscribe(results => {
            this.crashSeverity = results;
        });
        this.lookup.getWeatherCondition().subscribe(results => {
            this.weatherCondition = results;
        });

        // this.lookup.getBehavioralFactors().subscribe(results => {
        //    this.behavioralFactors = results;
        // });
        // this.lookup.getReportingAgencies().subscribe(results => {
        //    this.reportingAgency = results;
        // });

        this.cmvInvolved = [{ key: 'Y', name: 'Yes' }, { key: 'N', name: 'No' }];
        this.bikePedInvolved = [{ key: 'Bike', name: 'Bike Involved' }, { key: 'Ped', name: 'Pedestrian Involved' }];
        this.dayOrNight = [{ key: 'Day', name: 'Day' }, { key: 'Night', name: 'Night' }];
        this.intersectionRelated = [{ key: 'Y', name: 'Yes' }, { key: 'N', name: 'No' }];
        this.laneDeparture = [{ key: 'Y', name: 'Yes' }, { key: 'N', name: 'No' }];
        this.formType = [{ key: 'L', name: 'Long' }, { key: 'S', name: 'Short' }];
        this.codeable = [{ key: 'Y', name: 'Codeable' }];

    }

    public pageChange(event: PageChangeEvent) {
        this.state.crashGridSkip = event.skip;
        this.loadCrashAttributes();
    }

    private setInitialDateRange(serverDate: Date) {
        this.state.dateTimeScope.dateRange = {
            endDate: serverDate,
            startDate: moment(serverDate).subtract(6, 'days').toDate()
        };
    }

    private setInitialPlace() {
        // todo
    }

    private issueCrashQuery() {
        this.crashService
            .createCrashQuery(this.state.dateTimeScope, this.state.placeScope, this.state.crashQuery)
            .subscribe((queryRef: QueryRef) => {
                this.state.crashQueryRef = queryRef;
                this.state.crashGridSkip = 0;
                this.loadCrashAttributes();
                this.loadCrashPoints();
            });
    }

    private loadCrashAttributes() {
        let token = this.state.crashQueryRef.queryToken;
        let fromIndex = this.state.crashGridSkip;
        let toIndex = this.state.crashGridSkip + this.state.gridPageSize;
        let totalCount = this.state.crashQueryRef.totalCount;
        this.crashService
            .getCrashData(token, fromIndex, toIndex)
            .subscribe((results: CrashResult[]) => this.state.crashGridData = {
                data: results,
                total: totalCount
            });
    }

    private loadCrashPoints() {
        // todo
    }


}
