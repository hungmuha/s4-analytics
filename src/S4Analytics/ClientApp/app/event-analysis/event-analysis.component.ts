﻿import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';
import * as _ from 'lodash';
import { PageChangeEvent } from '@progress/kendo-angular-grid';
import { LookupService, LookupKeyAndName } from '../shared';
import {
    DateTimeScope, PlaceScope, QueryRef, CrashResult,
    CrashService, EventAnalysisStateService, EventFeatureSet
} from './shared';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {
    allRdSysIds: LookupKeyAndName[];
    allCrashTypes: LookupKeyAndName[];
    allCrashSeverity: LookupKeyAndName[];
    allWeatherCondition: LookupKeyAndName[];
    allCmvInvolved: [{ key: boolean, name: string }];
    allDayOrNight: [{ key: string, name: string }];
    allIntersectionRelated: [{ key: boolean, name: string }];
    allLaneDeparture: [{ name: string, items: any[] }];
    allFormType: [{ key: string, name: string }];
    allCodeableOnly: [{ key: boolean, name: string }];
    allBikePedInvolved: [{ key: string, name: string }];
    allBehavioralFactors: [{ key: string, name: string }];
    // reportingAgency: LookupKeyAndName[];

    gridPageSize = 10;
    crashGridSkip = 0;
    crashLoadExtent: { minX: number, minY: number, maxX: number, maxY: number };

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

    applyFilters() {
        console.log('apply');
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

    get bikePedInvolved(): string[] |undefined {
        if (!this.state.crashQuery.bikePedInvolved) {
            return;
        }

        let results: string[] = [];
        if (this.state.crashQuery.bikePedInvolved.bikeInvolved) {
            results.push('Bike');
        }
        if (this.state.crashQuery.bikePedInvolved.pedInvolved) {
            results.push('Ped');
        }
        return results;
    }

    get behavioralFactors(): string[] |undefined {
        if (!this.state.crashQuery.behavioralFactors) {
            return;
        }

        let results: string[] = [];
        if (this.state.crashQuery.behavioralFactors.alcohol) {
            results.push('Alcohol');
        }
        if (this.state.crashQuery.behavioralFactors.drugs) {
            results.push('Drugs');
        }
        if (this.state.crashQuery.behavioralFactors.distraction) {
            results.push('Distraction');
        }
        if (this.state.crashQuery.behavioralFactors.aggressiveDriving) {
            results.push('Aggressive');
        }
        return results;
    }

    get laneDeparture(): string[] |undefined {
        if (!this.state.crashQuery.laneDepartures) {
            return;
        }

        let result: string[] = [];
        if (this.state.crashQuery.laneDepartures.offRoadAll) {
            result.push('OffRoadAll');
        }
        if (this.state.crashQuery.laneDepartures.offRoadRollover) {
            result.push('OffRoadRollover');
        }
        if (this.state.crashQuery.laneDepartures.offRoadCollisionWithFixedObject) {
            result.push('OffRoadCollision');
        }
        if (this.state.crashQuery.laneDepartures.crossedIntoOncomingTraffic) {
            result.push('CrossedIntoTraffic');
        }
        if (this.state.crashQuery.laneDepartures.sideswipe) {
            result.push('Sideswipe');
        }
        return result;
    }

    set bikePedInvolved(results: string[] | undefined) {
        if (results && results.length > 0) {
            this.state.crashQuery.bikePedInvolved = {
                bikeInvolved: results.indexOf('Bike') >= 0 ? true : false,
                pedInvolved: results.indexOf('Ped') >= 0 ? true : false
            };
        }
        else {
            this.state.crashQuery.bikePedInvolved = undefined;
        }
    }

    set behavioralFactors(results: string[] | undefined) {
        if (results && results.length < 0) {
            this.state.crashQuery.behavioralFactors = {
                alcohol: results.indexOf('Alcohol') >= 0 ? true : false,
                drugs: results.indexOf('Drugs') >= 0 ? true : false,
                distraction: results.indexOf('Distraction') >= 0 ? true : false,
                aggressiveDriving: results.indexOf('Aggressive') >= 0 ? true : false,
            };
        }
        else {
            this.state.crashQuery.behavioralFactors = undefined;
        }
    }

    set laneDeparture(results: string[] |undefined) {
        if (results && results.length < 0) {
            this.state.crashQuery.laneDepartures = {
                offRoadAll: results.indexOf('OffRoadAll') >= 0 ? true : false,
                offRoadRollover: results.indexOf('OffRoadRollover') >= 0 ? true : false,
                offRoadCollisionWithFixedObject: results.indexOf('OffRoadCollision') >= 0 ? true : false,
                crossedIntoOncomingTraffic: results.indexOf('CrossedIntoTraffic') >= 0 ? true : false,
                sideswipe: results.indexOf('Sideswipe') >= 0 ? true : false
            };
        }
        else {
            this.state.crashQuery.laneDepartures = undefined;
        }
    }

    ngOnInit() {
        this.route.data
            .subscribe((data: { serverDate: Date }) => {
                // set initial date range; issue query
                this.setInitialDateRange(data.serverDate);
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
            this.allRdSysIds = results;
        });
        this.lookup.getCrashTypes().subscribe(results => {
            this.allCrashTypes = results;
        });
        this.lookup.getCrashSeverity().subscribe(results => {
            this.allCrashSeverity = results;
        });
        this.lookup.getWeatherCondition().subscribe(results => {
            this.allWeatherCondition = results;
        });

        // Need to finish implementation of heirarchial filter
        // this.lookup.getReportingAgencies().subscribe(results => {
        //    this.reportingAgency = results;
        // });

        // todo: make these string keys enums
        this.allBehavioralFactors = [
            { key: 'Aggressive', name: 'Aggressive Driving' },
            { key: 'Distracted', name: 'Distracted Driving' },
            { key: 'Alcohol', name: 'Alcohol Involved' },
            { key: 'Drugs', name: 'Drugs Involved' }];
        this.allLaneDeparture = [
            {
                name: 'Any', items: [
                    { key: 'OffRoadAll', name: 'Off Road - All' },
                    { key: 'OffRoadRollover', name: 'Off Road - Rollover' },
                    { key: 'OffRoadCollision', name: 'Off Road - Collision Fixed Object' },
                    { key: 'CrossedIntoTraffic', name: 'Crossed into Oncoming Traffic' },
                    { key: 'Sideswipe', name: 'Sideswipe' }
                ]
            }];
        this.allCmvInvolved = [{ key: true, name: 'Yes' }, { key: false, name: 'No' }];
        this.allBikePedInvolved = [{ key: 'Bike', name: 'Bike Involved' }, { key: 'Ped', name: 'Pedestrian Involved' }];
        this.allDayOrNight = [{ key: 'Day', name: 'Day' }, { key: 'Night', name: 'Night' }];
        this.allIntersectionRelated = [{ key: true, name: 'Yes' }, { key: false, name: 'No' }];
        this.allFormType = [{ key: 'L', name: 'Long' }, { key: 'S', name: 'Short' }];
        this.allCodeableOnly = [{ key: true, name: 'Codeable' }];
    }

    pageChange(event: PageChangeEvent) {
        this.crashGridSkip = event.skip;
        this.loadCrashAttributes();
    }

    extentChange(extent: ol.Extent) {
        // do nothing if there is no query ref or load extent (the page may still be loading)
        if (this.state.crashQueryRef === undefined || this.crashLoadExtent === undefined) {
            return;
        }

        // are we zooming outside of the latest load extent?
        let outsideQueryExtent =
            extent[0] < this.crashLoadExtent.minX ||
            extent[1] < this.crashLoadExtent.minY ||
            extent[2] > this.crashLoadExtent.maxX ||
            extent[3] > this.crashLoadExtent.maxY;

        // are we zooming inside the (sampled) feature extent?
        let isSample = this.state.crashFeatureSet.isSample;
        let insideSampledFeatureExtent = false;
        if (isSample && this.state.crashFeatureSet.featureExtent !== undefined) {
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

        if (this.state.crashQueryRef.totalCount === 0) {
            // no data exists for current query; create an empty data set and return
            this.state.crashGridData = { data: [], total: 0 };
            return;
        }

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

    private loadCrashPoints(mapExtent?: ol.Extent) {

        if (this.state.crashQueryRef.totalCount === 0) {
            // no data exists for current query; create an empty feature set and return
            this.state.crashFeatureSet = new EventFeatureSet(this.state.crashQueryRef.queryToken, 'crash');
            return;
        }

        let loadExtent: { minX: number, minY: number, maxX: number, maxY: number };

        if (mapExtent === undefined) {
            // new query was issued
            loadExtent = this.state.crashQueryRef.extent;
        }
        else {
            // map extent was changed
            loadExtent = { minX: mapExtent[0], minY: mapExtent[1], maxX: mapExtent[2], maxY: mapExtent[3] };
        }

        // retrieve a feature set
        this.crashService
            .getCrashFeatures(this.state.crashQueryRef.queryToken, loadExtent)
            .subscribe((featureSet: EventFeatureSet) => {
                this.state.crashFeatureSet = featureSet;
                this.crashLoadExtent = loadExtent;
            });
    }
}
