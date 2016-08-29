import { Observable } from 'rxjs/Observable';
import { PbcatInfo, PbcatPedestrianInfo, PbcatBicyclistInfo } from './pbcat-info';
import { PbcatStep } from './pbcat-step';
import { PbcatItem } from './pbcat-item';
import { PbcatConfig, PbcatScreenConfig, PbcatItemConfig } from './pbcat-config.d.ts';

export enum FlowType {
    Bicyclist,
    Pedestrian
}

export class PbcatFlow {
    public stepHistory: PbcatStep[] = [];
    public isFlowComplete: boolean = false;
    public hasValidState: boolean = true;
    private currentStepIndex: number = -1;

    constructor(
        private config: PbcatConfig,
        public flowType: FlowType,
        public hsmvReportNumber: number,
        public autoAdvance: boolean) {
    }

    get isFinalStep(): boolean {
        return this.isFlowComplete && this.currentStepIndex === this.stepHistory.length - 1;
    }

    get showSummary(): boolean {
        return this.currentStepIndex === this.stepHistory.length;
    }

    get currentStep(): PbcatStep {
        return this.stepHistory[this.currentStepIndex];
    }

    get previousStep(): PbcatStep {
        return this.stepHistory[this.currentStepIndex - 1];
    }

    get nextStep(): PbcatStep {
        return this.stepHistory[this.currentStepIndex + 1];
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

    get canProceed(): boolean {
        return !this.showSummary && this.currentStep && this.currentStep.selectedItem !== undefined;
    }

    get canGoBack(): boolean { return this.previousStep !== undefined; }

    get canReturnToSummary(): boolean {
        return this.isFlowComplete && !this.showSummary && !this.isFinalStep;
    }

    get pbcatInfo(): PbcatInfo {
        // mock logic to create pbcatInfo ...
        let info = this.flowType === FlowType.Pedestrian
            ? new PbcatPedestrianInfo()
            : new PbcatBicyclistInfo();
        for (let step of this.stepHistory) {
            if (step.selectedItem !== undefined) {
                (info as any)[step.infoAttrName] = step.selectedItem.infoAttrValue;
            }
        }
        return info;
    }

    goToStep(stepNumber: number) {
        let stepIndex = stepNumber - 1; // stepNumber is 1-based
        let isNewFlow = stepIndex === 0 && this.stepHistory.length === 0;
        let stepExists = stepIndex >= 0 && stepIndex < this.stepHistory.length;
        this.currentStepIndex = stepIndex;

        if (isNewFlow) {
            let currentStep = this.getFirstStep();
            this.stepHistory.push(currentStep);
        }
        else if (!stepExists) {
            this.hasValidState = false;
        }
    }

    goToSummary() {
        if (this.isFlowComplete) {
            this.currentStepIndex = this.stepHistory.length; // summary isn't actually in the stepHistory
        }
        else {
            this.hasValidState = false;
        }
    }

    selectItemForCurrentStep(item: PbcatItem) {
        let isCurrentSelectedItem = this.currentStep.selectedItem === item;

        if (!isCurrentSelectedItem) {
            // select the item
            this.currentStep.selectedItem = item;
            // clear the step history after this step
            this.stepHistory = this.stepHistory.slice(0, this.currentStepIndex + 1);
            // queue up the next step
            let nextStep = this.calculateNextStep();
            if (nextStep) {
                this.isFlowComplete = false;
                this.stepHistory.push(nextStep);
            }
            else {
                this.isFlowComplete = true;
            }
        }
    }

    private getFirstStep(): PbcatStep {
        let screenName = '1';
        let screenConfig = this.config[screenName];
        let step = new PbcatStep(screenConfig.title, screenConfig.description, screenConfig.infoAttrName);
        step.items = screenConfig.items.map((item, index) =>
            new PbcatItem(
                index, item.infoAttrValue, item.title,
                item.nextScreenName, item.description,
                item.imageUrl, false)
        );
        return step;
    }

    private calculateNextStep(): PbcatStep {
        let screenName: string;
        let step: PbcatStep;
        let screenConfig: PbcatScreenConfig;

        if (this.currentStep) {
            screenName = this.currentStep.selectedItem.nextScreenName;
        }
        if (screenName !== 'END') {
            screenConfig = this.config[screenName];
        }
        if (screenConfig) {
            step = new PbcatStep(screenConfig.title, screenConfig.description, screenConfig.infoAttrName);
            step.items = screenConfig.items.map((item, index) =>
                new PbcatItem(
                    index, item.infoAttrValue, item.title,
                    item.nextScreenName, item.description,
                    item.imageUrl, false)
            );
        }

        return step;
    }
}
