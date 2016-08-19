import { PbcatPedestrianInfo } from './pbcat-ped-info.model';
import { PbcatStep } from './pbcat-step.model';
import { PbcatItem } from './pbcat-item.model';
import { PbcatCrashType } from './pbcat-crash-type.model';
import { Observable } from 'rxjs/Observable';

class PbcatData {

    static getPbcatPedestrianInfo(hsmvReportNumber: number): PbcatPedestrianInfo {
        // GET /api/pbcat/ped/:hsmvRptNr
        return new PbcatPedestrianInfo();
    }

    static savePbcatPedestrianInfo(
        hsmvReportNumber: number,
        pedInfo: PbcatPedestrianInfo,
        getNextCrash: boolean = false): number {
        // POST /api/pbcat/ped
        //  PUT /api/pbcat/ped/:hsmvRptNr
        return getNextCrash ? hsmvReportNumber + 1 : undefined;
    }

    static deletePbcatPedestrianInfo(hsmvReportNumber: number): void {
        //  DELETE /api/pbcat/ped/:hsmvRptNr
    }

    static calculatePedestrianCrashType(pedInfo: PbcatPedestrianInfo): PbcatCrashType {
        // GET /api/pbcat/ped/crashtype
        let crashType = new PbcatCrashType();
        crashType.crashTypeNbr = 781;
        crashType.crashTypeDesc = "Motorist Left Turn - Parallel Paths";
        crashType.crashGroupNbr = 790;
        crashType.crashGroupDesc = "Crossing Roadway - Vehicle Turning";
        crashType.crashTypeExpanded = 12781;
        crashType.crashGroupExpanded = 12790;
        return crashType;
    }
}

export class PbcatFlow {
    public stepHistory: PbcatStep[] = [];
    public currentStep: PbcatStep;
    public previousStep: PbcatStep;
    public nextStep: PbcatStep;
    public autoAdvance: boolean = true;
    public showSummary: boolean = false;
    public isFinalStep: boolean = false;
    public isFlowComplete: boolean = false;
    public hasValidState: boolean = true;
    private pedInfo: PbcatPedestrianInfo;
    private currentStepIndex: number = -1;

    constructor(private pbcatConfig: Observable<any>, public hsmvReportNumber: number) {
        this.pedInfo = PbcatData.getPbcatPedestrianInfo(hsmvReportNumber);
    }

    // todo: upon item selection, determine next step and populate stepHistory

    goToStepNumber(stepNumber: number) {
        this.showSummary = false;
        if (stepNumber === 1 && this.stepHistory.length === 0) {
            let nextStep = this.getNextStep();
            if (nextStep) {
                this.nextStep = nextStep;
                this.stepHistory.push(nextStep);
            }
            else {
                this.hasValidState = false;
            }
        }
        let stepExists = stepNumber > 0 && stepNumber <= this.stepHistory.length;
        if (stepExists) {
            let stepIndex = stepNumber - 1; // stepNumber is 1-based
            this.currentStepIndex = stepIndex;
            this.previousStep = this.stepHistory[stepIndex - 1];
            this.currentStep = this.stepHistory[stepIndex];
            this.nextStep = this.stepHistory[stepIndex + 1];
            this.isFinalStep = !this.nextStep && this.isFlowComplete;
        }
        else {
            this.hasValidState = false;
        }
    }

    goToSummary() {
        if (this.isFlowComplete) {
            this.showSummary = true;
        }
        else {
            this.hasValidState = false;
        }
    }

    selectItemForCurrentStep(item: PbcatItem) {
        // set the selected item and queue up the next step
        let sameItem =
            this.currentStep.selectedItem &&
            this.currentStep.selectedItem.index === item.index;
        this.currentStep.selectedItem = item;
        if (!sameItem) {
            // clear the step history after this step
            delete this.nextStep;
            this.stepHistory = this.stepHistory.slice(0, this.currentStepIndex + 1);
            this.isFlowComplete = false;
            // get the next step
            let nextStep = this.getNextStep();
            if (nextStep) {
                this.nextStep = nextStep;
                this.stepHistory.push(nextStep);
            }
        }
    }

    updateInfoForSelectedItem() {
        // mock logic to update this.pedInfo ...
        // does not account for any steps removed from the history
        let pbcatItem = this.currentStep.selectedItem;
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
    }

    getNextStep(): PbcatStep {
        let nextStep: PbcatStep;

        // mock logic that assumes one possible sequence
        if (this.currentStepIndex === -1) {
            nextStep = new PbcatStep('Where did the crash occur?', 'pedestrianPositionCd');
        }
        else if (this.currentStepIndex === 0) {
            nextStep = new PbcatStep('What was the position of the pedestrian when struck?', 'motoristDirTravelCd');
        }
        else if (this.currentStepIndex === 1) {
            nextStep = new PbcatStep('Question 3', 'motoristManeuverCd');
        }
        else if (this.currentStepIndex === 2) {
            nextStep = new PbcatStep('Question 4', 'legOfIntrsectCd');
        }
        else if (this.currentStepIndex === 3) {
            nextStep = new PbcatStep('Question 5', 'pedestrianMovementCd');
        }
        else if (this.currentStepIndex === 4) {
            nextStep = new PbcatStep('Question 6', 'unusualCircumstancesCd');
        }
        else if (this.currentStepIndex === 5) {
            nextStep = new PbcatStep('Question 7', 'unusualVehicleTypeOrActionCd');
        }
        else if (this.currentStepIndex === 6) {
            nextStep = new PbcatStep('Question 8', 'unusualPedActionCd');
        }
        else if (this.currentStepIndex === 7) {
            nextStep = new PbcatStep('Question 9', 'typicalPedActionCd');
        }
        else if (this.currentStepIndex === 8) {
            nextStep = new PbcatStep('Question 10', 'crossingRoadwayCd');
        }
        else if (this.currentStepIndex === 9) {
            nextStep = new PbcatStep('Question 11', 'turnMergeCd');
        }

        if (nextStep === undefined) {
            this.isFlowComplete = true;
            this.isFinalStep = true;
        }
        else {
            // add four mock items
            nextStep.items = [1, 2, 3, 4].map(i => new PbcatItem(i, i, `Option ${i}`));
        }
        return nextStep;
    }

    saveAndComplete() {
        PbcatData.savePbcatPedestrianInfo(this.hsmvReportNumber, this.pedInfo);
    }

    saveAndNext(): number {
        let nextHsmvNumber = PbcatData.savePbcatPedestrianInfo(this.hsmvReportNumber, this.pedInfo, true);
        return nextHsmvNumber;
    }
}
