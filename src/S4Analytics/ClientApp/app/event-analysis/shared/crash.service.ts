import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import * as ol from 'openlayers';
import { EventPointCollection } from './event-point-collection';
import { CrashQuery } from './crash-query';

@Injectable()
export class CrashService {
    constructor(private http: Http) { }

    getCrashPoints(query: CrashQuery, extent: ol.Extent): Observable<EventPointCollection> {
        let minX = extent[0];
        let minY = extent[1];
        let maxX = extent[2];
        let maxY = extent[3];

        return this.http
            .post('api/crash/query', query)
            .map(response => response.headers.get('Location'))
            .switchMap(url => this.http.get(`${url}/point?minX=${minX}&minY=${minY}&maxX=${maxX}&maxY=${maxY}`))
            .map(response => response.json() as EventPointCollection);
    }
}
