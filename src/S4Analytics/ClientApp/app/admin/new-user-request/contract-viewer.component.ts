import { Component } from '@angular/core';
import { SafeResourceUrl } from '@angular/platform-browser';

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
    requestNumber: number;
    pdfUrl: SafeResourceUrl;

    constructor() {
    }

    ngOnInit() {
        // initalize option service here (note to self: see report-viewer.component.ts)
    }
}
