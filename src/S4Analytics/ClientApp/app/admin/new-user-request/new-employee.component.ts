import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../../keep-silverlight-alive.service';
import { OptionsService, Options } from '../../options.service';


export class NewUserRequestNewEmployeeComponent {

    constructor(
        private router: Router
    ) { }
}