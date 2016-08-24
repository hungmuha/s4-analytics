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

    groupedItems(groupCount: number): PbcatItem[][] {
        let groupedItems: PbcatItem[][] = [];
        let currGroup: PbcatItem[] = [];
        for (let item of this.step.items) {
            currGroup.push(item);
            if (currGroup.length === groupCount) {
                groupedItems.push(currGroup.map(item => item));
                currGroup = [];
            }
        }
        if (currGroup.length > 0) {
            groupedItems.push(currGroup.map(item => item));
        }
        return groupedItems;
    }

    hasImages(): boolean {
        return this.step.items[0].imageUrl !== undefined;
    }
}
