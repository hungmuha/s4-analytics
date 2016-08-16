import * as ng from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatStepComponent } from './pbcat-step.component';
import { PbcatSummaryComponent } from './pbcat-summary.component';
import { PbcatService, PbcatStep, ParticipantType } from './shared';

@ng.Component({
    selector: 'pbcat',
    template: require('./pbcat.component.html'),
    directives: [PbcatStepComponent, PbcatSummaryComponent],
    providers: [PbcatService]
})
export class PbcatComponent {
    private paramsSub: any;
    private stepNumber: number;
    private showSummary: boolean;
    private stepHistory: PbcatStep[];
    private currentStep: PbcatStep;
    private previousStep: PbcatStep;
    private nextStep: PbcatStep;
    private autoAdvance: boolean;
    private showReturnToSummary: boolean;
    private flowComplete: boolean;
    private hsmvReportNumber: number;
    private hsmvReportNumberDisplay: string;
    private participantType: ParticipantType;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) {
        this.stepHistory = [];
    }

    back() {
        // load the previous step in stepHistory
    }

    proceed() {
        // determine which step to show next, based on the selected item
        // if the user had navigated back and the next step differs from the
        // next step in stepHistory, clear all remaining items in stepHistory
        // and add the next step to stepHistory. set flowComplete to false
        // unless we are actually done. set currentStep, nextStep, and previousStep.

        // if we are done, set flowComplete and move to the summary instead.
    }

    returnToSummary() {
        // if flowComplete is true, go to the summary
    }

    saveAndClose() {
        // determine the type, save it, and close the window?
    }

    saveAndNext() {
        // determine the type, save it, and load another crash report
    }

    navigateToStep(stepNumber: any) {
        if (stepNumber === undefined) {
            this.showSummary = true;
        }
        else {
            this.stepNumber = Number(stepNumber);
            // if invalid step number, go back to the starting point (for now)
            if (isNaN(this.stepNumber)) {
                this.router.navigate(['pbcat']);
            }
        }
    }

    ngOnInit() {
        this.paramsSub = this.activatedRoute.params.subscribe(
            params => this.navigateToStep(params['currentStep'])
        );
        this.pbcatService.getPedestrianNextStep().then(step => undefined);
    }

    ngOnDestroy() {
        this.paramsSub.unsubscribe();
    }
}
