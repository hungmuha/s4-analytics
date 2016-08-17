import * as ng from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatService } from './shared';

@ng.Component({
    selector: 'pbcat-step',
    template: require('./pbcat-step.component.html')
})
export class PbcatStepComponent {
    // url parameter-driven props
    private paramsSub: any;
    private hsmvReportNumber: number;
    private stepNumber: number;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

    ngOnInit() {
        // subscribe to params
        this.paramsSub = this.activatedRoute.params.subscribe(
            params => this.processParams(params)
        );
    }

    ngOnDestroy() {
        // unsubscribe from params
        this.paramsSub.unsubscribe();
    }

    processParams(params: any) {
        this.hsmvReportNumber = +params['hsmvReportNumber'];
        this.stepNumber = +params['stepNumber'];
    }

}
