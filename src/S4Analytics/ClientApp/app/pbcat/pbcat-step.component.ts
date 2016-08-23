import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatService, PbcatStep, PbcatItem } from './shared';

@Component({
    selector: 'pbcat-step',
    template: require('./pbcat-step.component.html')
})
export class PbcatStepComponent {
    @Input() hsmvReportNumber: number;
    @Input() step: PbcatStep;
    @Input() stepNumber: number;
    @Output() selectItem = new EventEmitter<PbcatItem>();

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

    makeSelection(item: PbcatItem) {
        if (!item.notImplemented) {
            this.selectItem.emit(item);
        }
    }
}
