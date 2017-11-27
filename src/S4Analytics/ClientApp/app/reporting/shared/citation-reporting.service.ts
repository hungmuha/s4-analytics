import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ReportOverTime } from './report-over-time';
import { CitationsOverTimeQuery } from './citations-over-time-query';

@Injectable()
export class CitationReportingService {
    constructor(private http: Http) { }

    getCitationsOverTimeByYear(query: CitationsOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post('api/reporting/citation/year', query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCitationsOverTimeByMonth(year: number, query: CitationsOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post(`api/reporting/citation/${year}/month`, query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getCitationsOverTimeByDay(year: number, alignByWeek: boolean, query: CitationsOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post(`api/reporting/citation/${year}/day?alignByWeek=${alignByWeek}`, query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }
}