import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';

export class LookupKeyAndName {
    key: number;
    name: string;
}

@Injectable()
export class LookupService {

    constructor(private http: Http) { }

    getCounties(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/county')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getCities(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/city')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getGeographies(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/geography')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getReportingAgencies(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/agency')
            .map(response => response.json() as LookupKeyAndName[]);
    }
}
