// Service to retrieve server time from the API

import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ServerTimeResolveService implements Resolve<Date> {
    constructor(private http: Http) { }

    resolve(): Observable<Date> {
        let url = 'api/options/time';
        return this.http
            .get(url)
            .map(response => response.json() as { time: string })
            .map((obj: { time: string }) => new Date(obj.time));
    }
}
