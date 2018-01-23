import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LookupService, LookupKeyAndName } from '../shared';
import { DateTimeScope, PlaceScope, CrashQuery, QueryRef, CrashResult, CrashService, EventAnalysisStateService } from './shared';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {

    // modals store temporary values in these variables;
    // once applied the values are copied into the state
    _geoExtent: 'Statewide' | 'County' | 'City';
    _filteredCounties: LookupKeyAndName[];
    _filteredCities: LookupKeyAndName[];
    _selectedCounties: LookupKeyAndName[];
    _selectedCities: LookupKeyAndName[];

    constructor(
        private route: ActivatedRoute,
        private modalService: NgbModal,
        private crashService: CrashService,
        private state: EventAnalysisStateService,
        private lookup: LookupService) {
        this.state.geoExtent = 'Statewide';
        this.lookup.getCounties().subscribe(results => {
            this.state.allCounties = results;
            this._filteredCounties = [];
        });
        this.lookup.getCities().subscribe(results => {
            this.state.allCities = results;
            this._filteredCities = [];
        });
    }

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
        this.state.dateTimeScope = { ...value };
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

        let label: string = this.state.geoExtent;
        let items: string[] = [];
        switch (this.state.geoExtent) {
            case 'County':
                items = this.state.selectedCounties.map((item: LookupKeyAndName) => item.name);
                break;
            case 'City':
                items = this.state.selectedCities.map((item: LookupKeyAndName) => item.name);
                break;
            default:
                break;
        }
        if (items.length > 0) {
            label = `${label}: ${items.slice(0, 3).join(', ')}`;
            if (items.length > 3) {
                label = `${label}, +${items.length - 3}`;
            }
        }
        return label;
    }

    ngOnInit() {
        this.route.data
            .subscribe((data: { serverDate: Date }) => {
                // set initial date range; issue query
                this.setInitialDateRange(data.serverDate);
                this.issueCrashQuery();
            });
    }

    public filterCounties(filter: string): void {
        this._filteredCounties = filter.length > 0
            ? this.state.allCounties.filter(s => s.name.toLowerCase().indexOf(filter.toLowerCase()) !== -1).slice(0, 10)
            : [];
    }

    public filterCities(filter: string): void {
        this._filteredCities = filter.length > 0
            ? this.state.allCities.filter(s => s.name.toLowerCase().indexOf(filter.toLowerCase()) !== -1).slice(0, 10)
            : [];
    }

    public openPlaceModal(content: any) {
        // set temp vars from state vars
        this._geoExtent = this.state.geoExtent;
        this._selectedCounties = this.state.selectedCounties.slice();
        this._selectedCities = this.state.selectedCities.slice();

        this.modalService.open(content).result.then((result) => {
            // set state vars from temp vars
            this.state.geoExtent = this._geoExtent;
            this.state.selectedCounties = this._geoExtent === 'County'
                ? this._selectedCounties.slice()
                : [];
            this.state.selectedCities = this._geoExtent === 'City'
                ? this._selectedCities.slice()
                : [];
            this.issueCrashQuery();
        });
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

    private issueCrashQuery() {
        if (this.state.geoExtent === 'County') {
            this.state.placeScope.county = this.state.selectedCounties.map(c => c.key);
        }
        else if (this.state.geoExtent === 'City') {
            this.state.placeScope.city = this.state.selectedCities.map(c => c.key);
        }

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
