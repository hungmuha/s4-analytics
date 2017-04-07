import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { EventPointCollection } from './event-point-collection';

@Injectable()
export class CrashService {
    constructor(private http: Http) { }

    getCrashPoints(query: any): Observable<EventPointCollection> {
        return this.http
            .post('api/crash/query', query)
            .map(response => response.headers.get('Location'))
            .switchMap(url => this.http.get(`${url}/point?x1=488138.255108688&x2=625387.979093505&y1=607795.394428587&y2=646957.807445191`))
            .map(response => response.json() as EventPointCollection);
    }
}
