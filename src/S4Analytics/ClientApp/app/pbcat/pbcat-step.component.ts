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
    private _groupedItems: PbcatItem[][];
    private _placeholderArray: any[];

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

    get groupedItems(): PbcatItem[][] {
        // return items grouped in threes because of bootstrap card-deck behavior
        if (this._groupedItems === undefined) {
            this.groupItems();
        }
        return this._groupedItems;
    }

    get placeholderArray(): any[] {
        // return an array of placeholder items in case the last group contains
        // fewer than three items, because of bootstrap card-deck behavior
        // and because ngFor requires an array to loop over
        if (this._placeholderArray === undefined) {
            this.groupItems();
        }
        return this._placeholderArray;
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
        this._groupedItems = groupedItems;
        this._placeholderArray = new Array<any>(placeholderCount).fill(undefined);
    }
}
