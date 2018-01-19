// Service to retrieve server date from the API

import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ServerDateResolveService implements Resolve<Date> {
    constructor(private http: Http) { }

    resolve(): Observable<Date> {
        let url = 'api/options/date';
        return this.http
            .get(url)
            .map(response => response.json() as { date: string })
            .map((obj: { date: string }) => new Date(obj.date));
    }
}
