import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import * as _ from 'lodash';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { LookupService } from '../shared';
import {
    DateTimeScope, PlaceScope, QueryRef, CrashResult,
    CrashService, EventAnalysisStateService, EventFeatureSet
} from './shared';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {
    gridPageSize = 10;
    crashGridSkip = 0;

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

    ngOnInit() {
        this.route.data
            .subscribe((data: { serverDate: Date }) => {
                // set initial date range; issue query
                this.setInitialDateRange(data.serverDate);
                this.setInitialPlace();
                this.issueCrashQuery();
            });
        this.lookup.getCounties().subscribe(results => {
            this.state.allCounties = results;
        });
        this.lookup.getCities().subscribe(results => {
            this.state.allCities = results;
        });
    }

    pageChange(event: PageChangeEvent) {
        this.crashGridSkip = event.skip;
        this.loadCrashAttributes();
    }

    extentChange(extent: ol.Extent) {
        // do nothing if there is no query ref (the page may still be loading)
        if (this.state.crashQueryRef === undefined) {
            return;
        }

        // are we zooming outside of the latest query extent?
        let queryExtent = this.state.crashFeatureSet.queryExtent;
        let outsideQueryExtent =
            extent[0] < queryExtent.minX ||
            extent[1] < queryExtent.minY ||
            extent[2] > queryExtent.maxX ||
            extent[3] > queryExtent.maxY;

        // are we zooming inside the (sampled) feature extent?
        let isSample = this.state.crashFeatureSet.isSample;
        let insideSampledFeatureExtent = false;
        if (isSample) {
            let featureExtent = this.state.crashFeatureSet.featureExtent;
            insideSampledFeatureExtent =
                extent[0] > featureExtent.minX ||
                extent[1] > featureExtent.minY ||
                extent[2] < featureExtent.maxX ||
                extent[3] < featureExtent.maxY;
        }

        // if either case is true, reload points
        if (outsideQueryExtent || insideSampledFeatureExtent) {
            this.loadCrashPoints(extent);
        }
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
                this.crashGridSkip = 0;
                this.loadCrashAttributes();
                this.loadCrashPoints();
            });
    }

    private loadCrashAttributes() {
        let token = this.state.crashQueryRef.queryToken;
        let fromIndex = this.crashGridSkip;
        let toIndex = this.crashGridSkip + this.gridPageSize;
        let totalCount = this.state.crashQueryRef.totalCount;
        this.crashService
            .getCrashData(token, fromIndex, toIndex)
            .subscribe((results: CrashResult[]) => this.state.crashGridData = {
                data: results,
                total: totalCount
            });
    }

    private loadCrashPoints(extent?: ol.Extent) {
        let fitToFeatures: boolean;
        let queryExtent: { minX: number, minY: number, maxX: number, maxY: number };

        if (extent === undefined) {
            // new query was issued
            fitToFeatures = true;
            queryExtent = this.state.crashQueryRef.extent;
        }
        else {
            // map extent was changed
            fitToFeatures = false;
            queryExtent = { minX: extent[0], minY: extent[1], maxX: extent[2], maxY: extent[3] };
        }

        this.crashService
            .getCrashFeatures(this.state.crashQueryRef.queryToken, queryExtent)
            .subscribe((featureSet: EventFeatureSet) => {
                this.state.crashFeatureSet = featureSet;
                // todo: zoom to extent of points on initial load
            });
    }
}
