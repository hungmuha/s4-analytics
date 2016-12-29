import { Component } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../keep-silverlight-alive.service';
import { OptionsService, Options } from '../options.service';

@Component({
    template: require('./admin.component.html')
})
export class AdminComponent {}