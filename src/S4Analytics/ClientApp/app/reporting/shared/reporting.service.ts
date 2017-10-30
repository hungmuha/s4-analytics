import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ReportOverTimeByYear } from './report-over-time';

@Injectable()
export class ReportingService {
    constructor(private http: Http) { }

    getCrashesOverTime(): Observable<ReportOverTimeByYear> {
        return this.http
            .get('api/crash/report/year')
            .map(response => response.json() as ReportOverTimeByYear);
    }
}
