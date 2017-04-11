import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { Extent } from 'openlayers';
import { EventPointCollection } from './event-point-collection';
import { CrashQuery } from './crash-query';

@Injectable()
export class CrashService {
    constructor(private http: Http) { }

    getCrashPoints(query: CrashQuery, extent: Extent): Observable<EventPointCollection> {
        return this.http
            .post('api/crash/query', query)
            .map(response => response.headers.get('Location'))
            .switchMap(url => this.http.get(`${url}/point?x1=${extent[0]}&y1=${extent[1]}&x2=${extent[2]}&y2=${extent[3]}`))
            .map(response => response.json() as EventPointCollection);
    }
}
