import { Component, Input } from '@angular/core';
import { PbcatStep, PbcatCrashType } from './shared';

@Component({
    selector: 'pbcat-summary',
    template: require('./pbcat-summary.component.html')
})
export class PbcatSummaryComponent {
    @Input() hsmvReportNumber: number;
    @Input() stepHistory: PbcatStep[];
    @Input() crashType: PbcatCrashType;
}
