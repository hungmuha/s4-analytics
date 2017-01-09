import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SafeResourceUrl, DomSanitizer } from '@angular/platform-browser';
import { OptionsService } from '../../options.service';

// This component should not be necessary, but IE exhibits some
// buggy behavior when programatically controlling a child window
// that initially contains a PDF, as opposed to an HTML document.
// A simple IFRAME works around that buggy behavior and provides
// a cleaner URL to boot.

@Component({
    selector: 'contract-viewer',
    template: `<iframe class="contract-viewer" [src]="pdfUrl"></iframe>`
})
export class ContractViewerComponent {
    requestNumber: number = 85440392;
    pdfUrl: SafeResourceUrl;

    constructor(
        private route: ActivatedRoute,
        private sanitizer: DomSanitizer,
        private optionsService: OptionsService) {
    }

    ngOnInit() {
        this.requestNumber = +this.route.snapshot.params['requestNumber'];
        this.optionsService.getOptions()
            .first()
            .subscribe(options => {
                this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
                    `${options.silverlightBaseUrl}ImageHandler.ashx?hsmv=${this.requestNumber}`);
            });
    }
}
