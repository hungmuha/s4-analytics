import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { OptionsService, Options } from './options.service';
import './rxjs-operators';

@Injectable()
export class KeepSilverlightAliveService {
    constructor(
        private http: Http,
        private optionsService: OptionsService) { }

    keepAlive() {
        // keep the silverlight app alive
        if (opener && (opener as any).keepSilverlightAlive) {
            (opener as any).keepSilverlightAlive();
        }
        // keep the silverlight app server alive
        this.optionsService.getOptions()
            .first()
            .catch(err => Observable.throw(err))
            .subscribe(options => {
                this.callKeepAlivePage(options.silverlightBaseUrl);
            });
    }

    private callKeepAlivePage(silverlightBaseUrl: string) {
        let url = silverlightBaseUrl + 'KeepAlive.aspx';
        this.http.get(url)
            .first()
            .catch(err => Observable.throw(err))
            .subscribe();
    }
}
