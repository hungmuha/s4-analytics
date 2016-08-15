import * as ng from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatStepComponent, PbcatSummaryComponent } from './';

@ng.Component({
    selector: 'pbcat',
    template: require('./pbcat.component.html'),
    directives: [PbcatStepComponent, PbcatSummaryComponent]
})
export class PbcatComponent {
    private paramsSub: any;
    private stepNumber: number;
    private showSummary: boolean;

    constructor(private router: Router, private activatedRoute: ActivatedRoute) { }

    setCurrentStep(currentStep: any) {
        if (currentStep === undefined) {
            this.showSummary = true;
        }
        else {
            this.stepNumber = Number(currentStep);
            // if invalid step number, go back to the starting point (for now)
            if (isNaN(this.stepNumber)) {
                this.router.navigate(['pbcat']);
            }
        }
    }

    ngOnInit() {
        this.paramsSub = this.activatedRoute.params.subscribe(
            params => this.setCurrentStep(params['currentStep'])
        );
    }

    ngOnDestroy() {
        this.paramsSub.unsubscribe();
    }
}
