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
    // url parameter-driven props
    private paramsSub: any;
    private params: { [key: string]: any };
    private hsmvReportNumber: number;
    private stepNumber: number;
    private participantType: ParticipantType = ParticipantType.Pedestrian; // bicyclist not implemented in prototype

    // state props
    private hsmvReportNumberDisplay: string;
    private stepHistory: PbcatStep[] = [];
    private currentStep: PbcatStep;
    private previousStep: PbcatStep;
    private nextStep: PbcatStep;
    private autoAdvance: boolean = true;
    private showSummary: boolean = false;
    private showReturnToSummary: boolean = false;
    private flowComplete: boolean = false;

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
        let invalidParams = false;

        let hsmvReportNumber = params['hsmvReportNumber'];
        this.hsmvReportNumber = Number(hsmvReportNumber);
        if (isNaN(this.hsmvReportNumber)) {
            invalidParams = true;
        }

        let stepNumber = params['stepNumber'];
        if (stepNumber === undefined) {
            this.showSummary = true;
        }
        else {
            this.stepNumber = Number(stepNumber);
            if (isNaN(this.stepNumber)) {
                invalidParams = true;
            }
        }

        if (invalidParams) {
            // go home for now
            this.router.navigate(['home']);
        }
    }

    nextStepNumber(): number {
        return this.stepNumber + 1;
    }

    back() {
        // load the previous step in stepHistory
    }

    proceed() {
        this.router.navigate(['pbcat', this.hsmvReportNumber, 'step', this.nextStepNumber()]);
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
}
