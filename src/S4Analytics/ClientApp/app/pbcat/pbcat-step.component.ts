import { Component, Input, Output, EventEmitter } from '@angular/core';
import * as _ from 'lodash';
import { PbcatStep, PbcatItem } from './shared';

@Component({
    selector: 'pbcat-step',
    template: require('./pbcat-step.component.html')
})
export class PbcatStepComponent {
    @Input() hsmvReportNumber: number;
    @Input() step: PbcatStep;
    @Input() stepNumber: number;
    @Output() selectItem = new EventEmitter<PbcatItem>();
    private groupedItems: Array<PbcatItem[]>;
    private placeholderItems: any[];

    ngOnChanges() {
        if (this.hasImages) {
            this.groupItems();
        }
        else {
            delete this.groupedItems;
            delete this.placeholderItems;
        }
    }

    get hasImages(): boolean {
        let imageUrls = this.step.items[0].imageUrls;
        return imageUrls !== undefined && imageUrls.length > 0;
    }

    makeSelection(item: PbcatItem) {
        this.selectItem.emit(item);
    }

    groupItems(groupSize: number = 3) {
        // group items and populate the placeholder array
        this.groupedItems = _.chunk(this.step.items, groupSize);
        let placeholderCount = groupSize - _.last(this.groupedItems).length;
        this.placeholderItems = new Array<any>(placeholderCount).fill(undefined);
    }
}
