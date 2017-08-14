import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { AppStateService } from './app-state.service';
import '../rxjs-operators';
import * as moment from 'moment';

@Injectable()
export class KeepSilverlightAliveService {
    private nextCallTime: moment.Moment;
    private keepAliveInterval = 10; // in minutes

    constructor(
        private http: Http,
        private state: AppStateService) {
        // schedule the initial keep alive call
        this.nextCallTime = moment().add(this.keepAliveInterval, 'minutes');
    }

    keepAlive() {
        // keep the silverlight app alive
        if (opener && (opener as any).keepSilverlightAlive) {
            (opener as any).keepSilverlightAlive();
        }
        // keep the silverlight app server alive
        let now = moment();
        if (now.isAfter(this.nextCallTime)) {
            let url = `${this.state.options.silverlightBaseUrl}KeepAlive.aspx`;
            this.http.get(url).first().subscribe();
            this.nextCallTime = now.add(this.keepAliveInterval, 'minutes');
        }
    }
}
