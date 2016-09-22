import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { OptionsService, Options } from './options.service';
import './rxjs-operators';
import * as moment from 'moment';

@Injectable()
export class KeepSilverlightAliveService {
    private nextCallTime: moment.Moment;
    private keepAliveInterval = 10; // in minutes

    constructor(
        private http: Http,
        private optionsService: OptionsService) {
        // schedule the initial keep alive call
        let now = moment();
        this.nextCallTime = now.add(this.keepAliveInterval, 'minutes');
    }

    keepAlive() {
        // keep the silverlight app alive
        if (opener && (opener as any).keepSilverlightAlive) {
            (opener as any).keepSilverlightAlive();
        }
        // keep the silverlight app server alive
        let now = moment();
        if (now.isAfter(this.nextCallTime)) {
            this.optionsService.getOptions()
                .first()
                .catch(err => Observable.throw(err))
                .subscribe(options => {
                    this.callKeepAlivePage(options.silverlightBaseUrl);
                });
            this.nextCallTime = now.add(this.keepAliveInterval, 'minutes');
        }
    }

    private callKeepAlivePage(silverlightBaseUrl: string) {
        let url = silverlightBaseUrl + 'KeepAlive.aspx';
        this.http.get(url)
            .first()
            .catch(err => Observable.throw(err))
            .subscribe();
    }
}
