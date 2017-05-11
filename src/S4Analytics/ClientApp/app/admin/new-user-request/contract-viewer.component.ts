import { Component } from '@angular/core';


@Component({
    selector: 'contract-viewer',
    template: `<iframe class="contract-viewer" [src]="pdfUrl"></iframe>`
})
export class ContractViewerComponent {

    constructor()
    {}

}
