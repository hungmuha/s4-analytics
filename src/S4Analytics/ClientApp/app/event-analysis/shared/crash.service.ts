import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import * as ol from 'openlayers';
import { EventFeatureSet } from './event-feature-set';
import { DateTimeScope, PlaceScope, CrashQuery, QueryRef } from './crash-query';
import { CrashResult } from './crash-result';

@Injectable()
export class CrashService {
    constructor(private http: Http) { }

    createCrashQuery(
        dateTimeScope: DateTimeScope,
        placeScope: PlaceScope,
        query: CrashQuery
    ): Observable<QueryRef> {
        let url = 'api/crash/query';
        let scopeAndQuery = { ...dateTimeScope, ...placeScope, ...query };
        return this.http
            .post(url, scopeAndQuery)
            .map(response => response.json() as QueryRef);
    }

    getCrashFeatures(
        queryToken: string,
        extent: ol.Extent | { minX: number, minY: number, maxX: number, maxY: number }
    ): Observable<EventFeatureSet> {
        let minX: number, minY: number, maxX: number, maxY: number;
        [minX, minY, maxX, maxY] = Array.isArray(extent)
            ? extent
            : [extent.minX, extent.minY, extent.maxX, extent.maxY];
        let url = `api/crash/${queryToken}/feature?minX=${minX}&minY=${minY}&maxX=${maxX}&maxY=${maxY}`;
        return this.http
            .get(url)
            .map(response => response.json() as EventFeatureSet);
    }

    getCrashData(
        queryToken: string,
        fromIndex: number,
        toIndex: number
    ): Observable<Array<CrashResult>> {
        let url = `api/crash/${queryToken}?fromIndex=${fromIndex}&toIndex=${toIndex}`;
        return this.http
            .get(url)
            .map(response => response.json() as Array<CrashResult>)
            .map(results => results.map(result => new CrashResult(result)));
    }
}
