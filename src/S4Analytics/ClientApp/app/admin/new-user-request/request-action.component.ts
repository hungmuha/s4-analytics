import { Component } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../../keep-silverlight-alive.service';
import { OptionsService, Options } from '../../options.service';

@Component({
    template: require('./request-action.component.html')
})
export class RequestActionComponent {
    constructor(
    ) { }
}