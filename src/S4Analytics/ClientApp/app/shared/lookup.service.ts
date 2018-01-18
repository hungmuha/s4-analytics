import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class LookupService {

    constructor(private http: Http) { }

    getGeographies(): Observable<{ key: number, name: string }[]> {
        return this.http
            .get('api/lookup/geographies')
            .map(response => response.json());
    }

    getReportingAgencies(): Observable<{ key: number, name: string }[]> {
        return this.http
            .get('api/lookup/agencies')
            .map(response => response.json());
    }
}
