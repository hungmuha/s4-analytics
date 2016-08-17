import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PbcatService, PbcatStep, PbcatItem, ParticipantType, PbcatPedestrianInfo } from './shared';

@Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    // url parameter-driven props
    private paramsSub: any;
    private hsmvReportNumber: number;
    private stepNumber: number;
    private participantType: ParticipantType;

    // state props
    private pedInfo: PbcatPedestrianInfo;
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
        private pbcatService: PbcatService) {
        this.participantType = ParticipantType.Pedestrian; // bicyclist not implemented in prototype
    }

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
        let hsmvReportNumber = +params['hsmvReportNumber'];
        this.loadCrash(hsmvReportNumber);

        if (params['stepNumber'] === undefined) {
            this.showSummary = true;
        } else {
            let stepNumber = +params['stepNumber'];
            this.loadStep(stepNumber);
        }
    }

    loadCrash(hsmvReportNumber: number) {
        this.pedInfo = this.pbcatService.getPbcatPedestrianInfo(hsmvReportNumber);
        this.hsmvReportNumber = hsmvReportNumber;
    }

    loadStep(stepNumber: number) {
        if (this.stepNumber === undefined || stepNumber - this.stepNumber === 1) {
            // if step number is one greater than the last, ask the service for the next step
            this.currentStep = this.pbcatService.getPedestrianNextStep(this.pedInfo);
            this.stepHistory.push(this.currentStep);
        }
        else if (stepNumber > 0 && stepNumber <= this.stepHistory.length) {
            // if step number already exists in stepHistory, just grab that step
            this.currentStep = this.stepHistory[stepNumber - 1];
        }
        this.stepNumber = stepNumber;
    }

    selectItem(pbcatItem: PbcatItem) {
        console.log(this.currentStep.infoAttr);
        switch (this.currentStep.infoAttr) {
            case "pedestrianPositionCd":
                this.pedInfo.pedestrianPositionCd = pbcatItem.value;
                break;
            case "motoristDirTravelCd":
                this.pedInfo.motoristDirTravelCd = pbcatItem.value;
                break;
            case "motoristManeuverCd":
                this.pedInfo.motoristManeuverCd = pbcatItem.value;
                break;
            case "legOfIntrsectCd":
                this.pedInfo.legOfIntrsectCd = pbcatItem.value;
                break;
            case "pedestrianMovementCd":
                this.pedInfo.pedestrianMovementCd = pbcatItem.value;
                break;
            case "unusualCircumstancesCd":
                this.pedInfo.unusualCircumstancesCd = pbcatItem.value;
                break;
            case "unusualVehicleTypeOrActionCd":
                this.pedInfo.unusualVehicleTypeOrActionCd = pbcatItem.value;
                break;
            case "unusualPedActionCd":
                this.pedInfo.unusualPedActionCd = pbcatItem.value;
                break;
            case "typicalPedActionCd":
                this.pedInfo.typicalPedActionCd = pbcatItem.value;
                break;
            case "crossingRoadwayCd":
                this.pedInfo.crossingRoadwayCd = pbcatItem.value;
                break;
            case "turnMergeCd":
                this.pedInfo.turnMergeCd = pbcatItem.value;
                break;
            default:
                break;
        }
        this.proceed();
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
