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
    private options: Options;

    constructor(
        private http: Http,
        private optionsService: OptionsService) {
        // schedule the initial keep alive call
        this.nextCallTime = moment().add(this.keepAliveInterval, 'minutes');
        this.optionsService.getOptions()
            .first()
            .subscribe(options => this.options = options);
    }

    keepAlive() {
        // keep the silverlight app alive
        if (opener && (opener as any).keepSilverlightAlive) {
            (opener as any).keepSilverlightAlive();
        }
        // keep the silverlight app server alive
        if (this.options) {
            let now = moment();
            if (now.isAfter(this.nextCallTime)) {
                let url = `${this.options.silverlightBaseUrl}KeepAlive.aspx`;
                this.http.get(url).first().subscribe();
                this.nextCallTime = now.add(this.keepAliveInterval, 'minutes');
            }
        }
    }
}
