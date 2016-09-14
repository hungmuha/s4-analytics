﻿import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import './rxjs-operators';

export interface Options {
    version: string;
    baseUrl: string;
    silverlightBaseUrl: string;
}

@Injectable()
export class OptionsResolveService implements Resolve<Options> {
    private cachedOptions: Options;

    constructor(private http: Http) { }

    public resolve(): Observable<any> {
        if (this.cachedOptions === undefined) {
            let url = 'api/options';
            return this.http.get(url)
                .map(response => response.json().data as Options)
                .do(options => this.cachedOptions = options);
        }
        else {
            return Observable.of(this.cachedOptions);
        }
    }
}
