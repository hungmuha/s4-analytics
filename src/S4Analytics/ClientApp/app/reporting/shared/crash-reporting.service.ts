import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ReportOverTime } from './report-over-time';
import { CrashesOverTimeQuery } from './crashes-over-time-query';

@Injectable()
export class CrashReportingService {
    constructor(private http: Http) { }

    getCrashesOverTimeByYear(query: CrashesOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post('api/reporting/crash/year', query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCrashesOverTimeByMonth(year: number, query: CrashesOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post(`api/reporting/crash/${year}/month`, query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCrashesOverTimeByDay(year: number, alignByWeek: boolean, query: CrashesOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post(`api/reporting/crash/${year}/day?alignByWeek=${alignByWeek}`, query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getGeographies(): Observable<{ key: number, name: string }[]> {
        return this.http
            .get('api/reporting/crash/geographies')
            .map(response => response.json());
    }

    getReportingAgencies(): Observable<{ key: number, name: string }[]> {
        return this.http
            .get('api/reporting/crash/agencies')
            .map(response => response.json());
    }
}
