// Service to retrieve client configuration options from the API

import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import '../rxjs-operators';

export interface Options {
    version: string;
    baseUrl: string;
    silverlightBaseUrl: string;
    isDevelopment: boolean;
    coordinateSystems: {
        [key: string]: {
            type: string,
            epsgCode: string,
            proj4Def: string,
            mapExtent: { minX: number, minY: number, maxX: number, maxY: number }
        }
    };
}

@Injectable()
export class OptionsResolveService implements Resolve<Options> {
    constructor(private http: Http) { }

    resolve(): Observable<Options> {
        let url = 'api/options';
        return this.http.get(url)
            .map(response => response.json() as Options);
    }
}
