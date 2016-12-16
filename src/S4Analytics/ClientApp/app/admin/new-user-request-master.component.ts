import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../keep-silverlight-alive.service';
import { OptionsService, Options } from '../options.service';

@Component({
    selector: 'newuserrequestqueue',
    template: require('./new-user-request-master.component.html')
})
export class NewUserRequestMasterComponent {
    constructor(
        private router: Router
    ) { }

    private ProcessRequest(): void {
        let processRoute = ['/newuserrequestprocess'];
        this.router.navigate(processRoute);
    }
}
