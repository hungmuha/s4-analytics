import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import './rxjs-operators';

export interface Options {
    version: string;
    baseUrl: string;
    silverlightBaseUrl: string;
}

@Injectable()
export class OptionsService {
    private cachedOptions: Options;

    constructor(private http: Http) { }

    public getOptions(): Observable<Options> {
        if (this.cachedOptions === undefined) {
            var url = 'api/options';
            return this.http.get(url)
                .map(response => response.json().data as Options)
                .do(options => this.cachedOptions = options);
        }
        else {
            return Observable.of(this.cachedOptions);
        }
    }
}
