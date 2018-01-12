import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ReportOverTime } from './report-over-time';
import { CitationsOverTimeQuery } from './citations-over-time-query';

@Injectable()
export class CitationReportingService {
    constructor(private http: Http) { }

    getMaxEventYear(): Observable<number> {
        return this.http
            .get('api/reporting/citation/max-event-year')
            .map(response => response.json() as number);
    }

    getMaxLoadYear(): Observable<number> {
        return this.http
            .get('api/reporting/citation/max-load-year')
            .map(response => response.json() as number);
    }

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

    getCitationsOverTimeByAttribute(year: number, attrName: string, query: CitationsOverTimeQuery): Observable<ReportOverTime> {
        return this.http
            .post(`api/reporting/citation/${year}/${attrName}`, query)
            .map(response => response.json() as ReportOverTime)
            .map(report => new ReportOverTime(report));
    }

    getGeographies(): Observable<{ key: number, name: string }[]> {
        return this.http
            .get('api/reporting/citation/geographies')
            .map(response => response.json());
    }

    getReportingAgencies(): Observable<{ key: number, name: string }[]> {
        return this.http
            .get('api/reporting/citation/agencies')
            .map(response => response.json());
    }
}