import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ReportOverTime } from './report-over-time';

@Injectable()
export class ReportingService {
    constructor(private http: Http) { }

    getCrashesOverTimeByYear(): Observable<ReportOverTime> {
        return this.http
            .get('api/crash/report/year')
            .map(response => response.json() as ReportOverTime);
    }

    getCrashesOverTimeByMonth(): Observable<ReportOverTime> {
        return this.http
            .get('api/crash/report/2017/month')
            .map(response => response.json() as ReportOverTime);
    }

    getCrashesOverTimeByDay(): Observable<ReportOverTime> {
        return this.http
            .get('api/crash/report/2017/day')
            .map(response => response.json() as ReportOverTime);
    }
}
