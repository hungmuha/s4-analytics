import * as ng from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatService, PbcatStep, ParticipantType } from './shared';

@ng.Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    // url parameter-driven props
    private paramsSub: any;
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
        this.hsmvReportNumber = +params['hsmvReportNumber'];
        this.stepNumber = +params['stepNumber'];
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
