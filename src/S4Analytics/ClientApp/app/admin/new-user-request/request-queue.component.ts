import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../../keep-silverlight-alive.service';
import { OptionsService, Options } from '../../options.service';
import { NewUserRequestStateService, NewUserRequestService } from '../shared';

@Component({
    template: require('./request-queue.component.html')
})
export class RequestQueueComponent {

    constructor(
        private state: NewUserRequestStateService,
        private newUserRequestService: NewUserRequestService,
        private modalService: NgbModal) {
    }

    ngOnInit() {
        this.state.newUserRequests = this.newUserRequestService.getNewUserRequests();
    }

    private sortColumn(): void { }


}
