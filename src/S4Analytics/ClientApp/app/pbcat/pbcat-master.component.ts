import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
    PbcatService,
    PbcatFlow,
    PbcatStep,
    PbcatItem,
    ParticipantType,
    PbcatPedestrianInfo
} from './shared';

@Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    private paramsSub: any;
    private hsmvReportNumber: number;
    private stepNumber: number;
    private pbcatFlow: PbcatFlow;
    private routeError: boolean = false;

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
        if (this.stepNumber) {
            this.pbcatFlow = this.pbcatService.getPbcatFlowAtStep(this.hsmvReportNumber, this.stepNumber);
        }
        else {
            this.pbcatFlow = this.pbcatService.getPbcatFlowAtSummary(this.hsmvReportNumber);
        }
        this.routeError = !this.pbcatFlow.hasValidState;
    }

    showSummary() {
        return this.pbcatFlow.showSummary;
    }

    currentStep() {
        return this.pbcatFlow.currentStep;
    }

    stepHistory() {
        return this.pbcatFlow.stepHistory;
    }

    selectItem(pbcatItem: PbcatItem) {
        this.pbcatFlow.selectItemForCurrentStep(pbcatItem);
        this.proceed();
    }

    back() {
        let previousStepNumber = this.stepNumber - 1;
        this.router.navigate(['pbcat', this.hsmvReportNumber, 'step', previousStepNumber]);
    }

    proceed() {
        let nextStepNumber = this.stepNumber + 1;
        if (nextStepNumber <= 11) {
            this.router.navigate(['pbcat', this.hsmvReportNumber, 'step', nextStepNumber]);
        }
        else {
            this.goToSummary();
        }
    }

    goToSummary() {
        this.router.navigate(['pbcat', this.hsmvReportNumber, 'summary']);
    }

    saveAndClose() {
        this.pbcatFlow.saveAndComplete();
    }

    saveAndNext() {
        let hsmvReportNumber = this.pbcatFlow.saveAndNext();
        this.router.navigate(['pbcat', hsmvReportNumber, 'step', 1]);
    }
}
