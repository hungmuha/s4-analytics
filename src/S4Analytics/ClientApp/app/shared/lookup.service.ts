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

    getRoadSystemIdentifiers(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/road-sys-id')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getCrashTypes(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/crash-type')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getCrashSeverity(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/crash-severity')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getWeatherCondition(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/weather-condition')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getBehavioralFactors(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/behavioral-factors')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getCmvInvolved(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/cmv-involved')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getBikeInvolved(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/bike-involved')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getPedInvolved(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/ped-involved')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getIntersectionRelated(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/intersection-related')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getDayOrNight(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/day-or-night')
            .map(response => response.json() as LookupKeyAndName[]);
    }

    getLaneDeparture(): Observable<LookupKeyAndName[]> {
        return this.http
            .get('api/lookup/lane-departure')
            .map(response => response.json() as LookupKeyAndName[]);
    }

}
