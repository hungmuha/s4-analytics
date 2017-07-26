import { Component } from '@angular/core';
import { AppStateService } from './app-state.service';

@Component({
    templateUrl: './analytics.component.html'
})
export class AnalyticsComponent {
    constructor(private state: AppStateService) { }
}
