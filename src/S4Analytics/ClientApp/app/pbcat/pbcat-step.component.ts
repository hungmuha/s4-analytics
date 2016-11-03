import { Component, Input, Output, EventEmitter, AfterViewChecked } from '@angular/core';
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
    private groupedItems: Array<PbcatItem[]>;
    private placeholderItems: any[];

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

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

    groupItems() {
        // group items by threes and populate the placeholder array
        let groupSize = 3;
        let groupedItems = new Array<PbcatItem[]>();
        let placeholderCount = 0;
        for (let i = 0; i < this.step.items.length; i += groupSize) {
            let items = this.step.items.slice(i, i + groupSize);
            groupedItems.push(items);
            placeholderCount = groupSize - items.length;
        }
        this.groupedItems = groupedItems;
        this.placeholderItems = new Array<any>(placeholderCount).fill(undefined);
    }
}
