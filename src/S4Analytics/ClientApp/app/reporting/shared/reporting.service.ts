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

    getCrashesOverTimeByMonth(year: number): Observable<ReportOverTime> {
        return this.http
            .get(`api/report/crash/${year}/month`)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCrashesOverTimeByDay(year: number, alignByWeek: boolean): Observable<ReportOverTime> {
        return this.http
            .get(`api/report/crash/${year}/day?alignByWeek=${alignByWeek}`)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }
}
