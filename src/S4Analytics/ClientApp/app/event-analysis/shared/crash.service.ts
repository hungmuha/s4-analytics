import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import * as _ from 'lodash';
import * as ol from 'openlayers';
import { EventFeatureSet } from './event-feature-set';
import { CrashQuery } from './crash-query';

@Injectable()
export class CrashService {
    constructor(private http: Http) { }

    // todo: decouple calls to create query

    getCrashFeatures(query: CrashQuery, extent: ol.Extent): Observable<EventFeatureSet> {
        let minX = extent[0];
        let minY = extent[1];
        let maxX = extent[2];
        let maxY = extent[3];

        return this.http
            .post('api/crash/query', query)
            .map(response => response.headers && response.headers.get('Location'))
            .switchMap(url => {
                if (!url) { return Observable.throw('Query URL missing'); }
                url = _.replace(url, /^.*api\//, 'api/'); // convert to relative url
                return this.http.get(`${url}/feature?minX=${minX}&minY=${minY}&maxX=${maxX}&maxY=${maxY}`);
            })
            .map(response => response.json() as EventFeatureSet);
    }

    getCrashData(query: CrashQuery, fromIndex: number, toIndex: number): Observable<any> {
        // todo: implement CrashResult type
        return this.http
            .post('api/crash/query', query)
            .map(response => response.headers && response.headers.get('Location'))
            .switchMap(url => {
                if (!url) { return Observable.throw('Query URL missing'); }
                url = _.replace(url, /^.*api\//, 'api/'); // convert to relative url
                return this.http.get(`${url}?fromIndex=${fromIndex}&toIndex=${toIndex}`);
            })
            .map(response => response.json());
    }
}
