import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SafeResourceUrl, DomSanitizationService } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';
import { Options } from './options-resolve.service';

// This component should not be necessary, but IE exhibits some
// buggy behavior when programatically controlling a child window
// that initially contains a PDF, as opposed to an HTML document.
// A simple IFRAME works around that buggy behavior and provides
// a cleaner URL to boot.

@Component({
    selector: 'report-viewer',
    template: `<iframe class="report-viewer" [src]="pdfUrl"></iframe>`
})
export class ReportViewerComponent {
    dataSub: Subscription;
    hsmvReportNumber: number;
    pdfUrl: SafeResourceUrl;

    constructor(
        private route: ActivatedRoute,
        private sanitizer: DomSanitizationService) {
    }

    ngOnInit() {
        this.hsmvReportNumber = +this.route.snapshot.params['hsmvReportNumber'];
        this.dataSub = this.route.data.subscribe(data => {
            let options = data['options'] as Options;
            this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
                `${options.silverlightBaseUrl}ImageHandler.ashx?hsmv=${this.hsmvReportNumber}`);
        });
    }

    ngOnDestroy() {
        this.dataSub.unsubscribe();
    }
}
