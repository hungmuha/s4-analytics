import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import '../rxjs-operators';

export interface Options {
    version: string;
    baseUrl: string;
    silverlightBaseUrl: string;
    contractShareUrl: string;
    contactShareUserName: string;
    contractSharePassword: string;
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
export class OptionsService {
    private cachedOptions: Options;

    constructor(private http: Http) { }

    public getOptions(): Observable<Options> {
        if (this.cachedOptions === undefined) {
            let url = 'api/options';
            return this.http.get(url)
                .map(response => response.json() as Options)
                .do(options => this.cachedOptions = options);
        }
        else {
            return Observable.of(this.cachedOptions);
        }
    }
}
