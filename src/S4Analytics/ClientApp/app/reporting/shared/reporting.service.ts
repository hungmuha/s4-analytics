import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ReportOverTime } from './report-over-time';

@Injectable()
export class ReportingService {
    constructor(private http: Http) { }

    getCrashesOverTimeByYear(): Observable<ReportOverTime> {
        return this.http
            .get('api/report/crash/year')
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCrashesOverTimeByMonth(year: number, yearOnYear: boolean): Observable<ReportOverTime> {
        return this.http
            .get(`api/report/crash/${year}/month?yearOnYear=${yearOnYear}`)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCrashesOverTimeByDay(year: number, yearOnYear: boolean, alignByWeek: boolean): Observable<ReportOverTime> {
        return this.http
            .get(`api/report/crash/${year}/day?yearOnYear=${yearOnYear}&alignByWeek=${alignByWeek}`)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }
}
