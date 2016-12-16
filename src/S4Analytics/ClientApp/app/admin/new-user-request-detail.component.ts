import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../keep-silverlight-alive.service';
import { OptionsService, Options } from '../options.service';

@Component({
    selector: 'newuserrequestprocess',
    template: require('./new-user-request-detail.component.html')
})
export class NewUserRequestDetailComponent {
    constructor(
        private router: Router
    ) { }
}