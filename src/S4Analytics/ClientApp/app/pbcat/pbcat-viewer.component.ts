import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SafeResourceUrl, DomSanitizationService } from '@angular/platform-browser';
import { Subscription } from 'rxjs/Subscription';
import { AppState } from '../app.state';

// This component should not be necessary, but IE exhibits some
// buggy behavior when programatically controlling a child window
// that initially contains a PDF, as opposed to an HTML document.
// A simple IFRAME works around that buggy behavior and provides
// a cleaner URL to boot.

@Component({
    selector: 'pbcat-viewer',
    template: require('./pbcat-viewer.component.html')
})
export class PbcatViewerComponent {
    paramSub: Subscription;
    pdfUrl: SafeResourceUrl;

    constructor(
        private route: ActivatedRoute,
        private sanitizer: DomSanitizationService,
        private appState: AppState) { }

    ngOnInit() {
        this.paramSub = this.route.params.subscribe(params => {
            let hsmvReportNumber = params['hsmvReportNumber'];
            let baseUrl = this.appState.options['silverlightBaseUrl'];
            this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
                `${baseUrl}ImageHandler.ashx?hsmv=${hsmvReportNumber}`);
        });
    }

    ngOnDestroy() {
        this.paramSub.unsubscribe();
    }
}
