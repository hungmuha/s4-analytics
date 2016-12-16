import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../keep-silverlight-alive.service';
import { OptionsService, Options } from '../options.service';

@Component({
    selector: 'admin',
    template: require('./admin.component.html')
})
export class AdminComponent {
    constructor(
        private router: Router
    ) { }
}
