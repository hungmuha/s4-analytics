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
        console.log(scopeAndQuery);
        return this.http
            .post(url, scopeAndQuery)
            .map(response => response.json() as QueryRef);
    }

    getCrashFeatures(
        queryToken: string,
        extent: ol.Extent
    ): Observable<EventFeatureSet> {
        let minX = extent[0];
        let minY = extent[1];
        let maxX = extent[2];
        let maxY = extent[3];
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
            .map(response => response.json() as Array<CrashResult>);
        // todo: convert date values
    }
}
