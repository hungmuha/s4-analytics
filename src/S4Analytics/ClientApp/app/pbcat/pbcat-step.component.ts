import { Component, Input, Output, EventEmitter, AfterViewChecked } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatService, PbcatStep, PbcatItem } from './shared';
import * as $ from 'jquery';

@Component({
    selector: 'pbcat-step',
    template: require('./pbcat-step.component.html')
})
export class PbcatStepComponent implements AfterViewChecked {
    @Input() hsmvReportNumber: number;
    @Input() step: PbcatStep;
    @Input() stepNumber: number;
    @Output() selectItem = new EventEmitter<PbcatItem>();

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

    ngAfterViewChecked() {
        if (this.hasImages) {
            this.setEqualThumbnailHeights();
        }
    }

    makeSelection(item: PbcatItem) {
        this.selectItem.emit(item);
    }

    get hasImages(): boolean {
        let imageUrls = this.step.items[0].imageUrls;
        return imageUrls !== undefined && imageUrls.length > 0;
    }

    setEqualThumbnailHeights() {
        // http://stackoverflow.com/questions/11071314/javascript-execute-after-all-images-have-loaded
        // http://stackoverflow.com/questions/9278569/equals-heights-of-thumbnails

        let images = document.images;
        let imageCount = images.length;
        let loadedCount = 0;

        let setEqualHeights = (group: any) => {
            let tallest = 0;
            group.each(function () {
                let thisHeight = $(this).height();
                if (thisHeight > tallest) {
                    tallest = thisHeight;
                }
            });
            group.each(function () { $(this).height(tallest); });
        };

        let incrementCounter = () => {
            loadedCount++;
            if (loadedCount === imageCount) {
                setEqualHeights($('.thumbnail'));
            }
        };

        [].forEach.call(images, function (img: any) {
            img.addEventListener('load', incrementCounter, false);
        });
    }
}
