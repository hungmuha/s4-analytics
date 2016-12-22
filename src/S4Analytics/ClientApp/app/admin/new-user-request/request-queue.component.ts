import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../../keep-silverlight-alive.service';
import { OptionsService, Options } from '../../options.service';

@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {

    private newUserRequests: Array<any>  = [
        { requestNbr: 1234, requestDt: '12/12/2017', requestType: 'abc', requestFirstName:
            'Sara Yorty', requestStatus: 'New user'
        },
        { requestNbr: 1235, requestDt: '12/14/2017', requestType: 'xyx', requestFirstName:
            'Jim Jones', requestStatus: 'New Agency'
        }
    ];

    private sortColumn(): void {

        this.newUserRequests = [
            {
                requestNbr: 1235, requestDt: '12/14/2017', requestType: 'xyx', requestFirstName:
                'Jim Jones', requestStatus: 'New Agency'
            },
            {
                requestNbr: 1234, requestDt: '12/12/2017', requestType: 'abc', requestFirstName:
                'Sara Yorty', requestStatus: 'New user'
            }
        ];
    }
}
