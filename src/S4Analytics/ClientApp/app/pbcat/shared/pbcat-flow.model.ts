import { Observable } from 'rxjs/Observable';
import { PbcatPedestrianInfo } from './pbcat-ped-info.model';
import { PbcatStep } from './pbcat-step.model';
import { PbcatItem } from './pbcat-item.model';
import { PbcatConfig, PbcatScreenConfig, PbcatItemConfig } from './pbcat-config.d.ts';

export class PbcatFlow {
    public stepHistory: PbcatStep[] = [];
    public currentStep: PbcatStep;
    public previousStep: PbcatStep;
    public nextStep: PbcatStep;
    public showSummary: boolean = false;
    public isFinalStep: boolean = false;
    public isFlowComplete: boolean = false;
    public hasValidState: boolean = true;
    private currentStepIndex: number = -1;

    constructor(
        private config: PbcatConfig,
        public hsmvReportNumber: number,
        public autoAdvance: boolean) {
    }

    get currentStepNumber(): number {
        return this.currentStepIndex + 1;
    }

    get previousStepNumber(): number {
        return this.previousStep ? this.currentStepNumber - 1 : undefined;
    }

    get nextStepNumber(): number {
        return this.nextStep ? this.currentStepNumber + 1 : undefined;
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
            // set item.selected
            for (let currItem of this.currentStep.items) {
                currItem.selected = currItem.index === item.index;
            }
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

    getPedInfo() {
        // mock logic to create pedInfo ...
        let pedInfo = new PbcatPedestrianInfo();
        for (let step of this.stepHistory) {
            if (step.selectedItem !== undefined) {
                (pedInfo as any)[step.infoAttrName] = step.selectedItem.infoAttrValue;
            }
        }
        return pedInfo;
    }

    getNextStep(): PbcatStep {
        let nextScreenName: string = "1";
        let nextStep: PbcatStep;

        if (this.currentStepIndex >= 0) {
            nextScreenName = this.currentStep.selectedItem.nextScreenName;
        }
        let nextScreenConfig: PbcatScreenConfig;
        // todo: eliminate this magic string
        if (nextScreenName !== 'END') {
            nextScreenConfig = this.config[nextScreenName];
        }

        if (nextScreenConfig === undefined) {
            this.isFlowComplete = true;
            this.isFinalStep = true;
        }
        else {
            // todo: set selected parameter of PbcatItem
            nextStep = new PbcatStep(nextScreenConfig.title, nextScreenConfig.description, nextScreenConfig.infoAttrName);
            nextStep.items = nextScreenConfig.items.map((item, index) =>
                new PbcatItem(
                    index,
                    item.infoAttrValue,
                    item.title,
                    item.nextScreenName,
                    item.description,
                    item.imageUrl,
                    false)
            );
        }

        return nextStep;
    }

    get canProceed() {
         // this logic is questionable ...
        return !this.showSummary && ((this.isFinalStep && this.isFlowComplete) || this.nextStep !== undefined);
    }

    get canGoBack() { return this.previousStep !== undefined; }

    get canReturnToSummary() { return this.isFlowComplete && !this.showSummary; }
}
