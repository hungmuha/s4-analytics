import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    templateUrl: './reporting.component.html'
})
export class ReportingComponent {
    constructor(private router: Router) { }

    get currentMode(): string {
        switch (this.router.url) {
            case '/reporting/crashes-over-time':
                return 'Crashes';
            case '/reporting/citations-over-time':
                return 'Citations';
            default:
                return '';
        }
    }
}
