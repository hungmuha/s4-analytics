import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../../keep-silverlight-alive.service';
import { OptionsService, Options } from '../../options.service';
import { NewUserRequestStateService } from './shared/new-user-request-state.service';
import { NewUserRequestService } from './shared/new-user-request.service';


@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestState: NewUserRequestService)
    {
        this.state.newUserRequests = this.newUserRequestState.getNewUserRequests();
    }

    private sortColumn(): void {



    }
}
