import { PbcatInfo, PbcatPedestrianInfo, PbcatBicyclistInfo } from './pbcat-info';
import { PbcatStep } from './pbcat-step';
import { PbcatItem } from './pbcat-item';
import { PbcatCrashType } from './pbcat-crash-type';
import { PbcatConfig, PbcatScreenConfig, PbcatItemConfig } from './pbcat-config.d.ts';

export enum FlowType {
    Bicyclist,
    Pedestrian
}

export class PbcatFlow {
    crashType: PbcatCrashType;
    isSaved: boolean = false;
    private _currentStepIndex: number = -1;
    private _stepHistory: PbcatStep[] = [];
    private _isFlowComplete: boolean = false;
    private _hasValidState: boolean = true;
    private _pbcatInfo: PbcatInfo;

    constructor(
        public flowType: FlowType,
        public hsmvReportNumber: number,
        public typingExists: boolean,
        pbcatInfo: PbcatInfo,
        private config: PbcatConfig) {
        this._pbcatInfo = pbcatInfo;
    }

    get stepHistory() { return this._stepHistory; }

    get isFlowComplete() { return this._isFlowComplete; }

    get hasValidState() { return this._hasValidState; }

    get isFinalStep(): boolean {
        return this._isFlowComplete && this._currentStepIndex === this._stepHistory.length - 1;
    }

    get showSummary(): boolean {
        return this._currentStepIndex === this._stepHistory.length;
    }

    get currentStep(): PbcatStep {
        return this._stepHistory[this._currentStepIndex];
    }

    get previousStep(): PbcatStep {
        return this._stepHistory[this._currentStepIndex - 1];
    }

    get nextStep(): PbcatStep {
        return this._stepHistory[this._currentStepIndex + 1];
    }

    get currentStepNumber(): number {
        return this._currentStepIndex + 1;
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
        return this._isFlowComplete && !this.showSummary && !this.isFinalStep;
    }

    get pbcatInfo(): PbcatInfo {
        let info = this._pbcatInfo;
        // mock logic to update pbcatInfo ...
        for (let step of this._stepHistory) {
            if (step.selectedItem !== undefined) {
                (info as any)[step.infoAttrName] = step.selectedItem.enumValue;
            }
        }
        return info;
    }

    goToStep(stepNumber: number) {
        let stepIndex = stepNumber - 1; // stepNumber is 1-based
        let isNewFlow = stepIndex === 0 && this._stepHistory.length === 0;
        let stepExists = stepIndex >= 0 && stepIndex < this._stepHistory.length;
        this._currentStepIndex = stepIndex;

        if (isNewFlow) {
            let currentStep = this.getFirstStep();
            this._stepHistory.push(currentStep);
        }
        else if (!stepExists) {
            this._hasValidState = false;
        }
    }

    goToSummary() {
        if (this._isFlowComplete) {
            this._currentStepIndex = this._stepHistory.length; // summary isn't actually in the stepHistory
        }
        else {
            this._hasValidState = false;
        }
    }

    selectItemForCurrentStep(item: PbcatItem) {
        let isCurrentSelectedItem = this.currentStep.selectedItem === item;

        if (!isCurrentSelectedItem) {
            // select the item
            this.currentStep.selectedItem = item;
            // clear the step history after this step
            this._stepHistory = this._stepHistory.slice(0, this._currentStepIndex + 1);
            // queue up the next step
            let nextStep = this.calculateNextStep();
            if (nextStep) {
                this._isFlowComplete = false;
                this._stepHistory.push(nextStep);
            }
            else {
                this._isFlowComplete = true;
            }
        }
    }

    private getFirstStep(): PbcatStep {
        let screenName = '1';
        let screenConfig = this.config[screenName];
        let step = new PbcatStep(screenConfig.title, screenConfig.description, screenConfig.infoAttrName, screenConfig.enumName);
        step.items = screenConfig.items.map((item, index) => this.itemFromItemConfig(item, index, false));
        return step;
    }

    private calculateNextStep(): PbcatStep {
        let screenName: string;
        let step: PbcatStep;
        let screenConfig: PbcatScreenConfig;

        if (this.currentStep) {
            screenName = this.currentStep.selectedItem.nextScreenName;
        }
        if (screenName) {
            screenConfig = this.config[screenName];
        }
        if (screenConfig) {
            step = new PbcatStep(screenConfig.title, screenConfig.description, screenConfig.infoAttrName, screenConfig.enumName);
            step.items = screenConfig.items.map((item, index) => this.itemFromItemConfig(item, index, false));
        }

        return step;
    }

    private itemFromItemConfig(itemConfig: PbcatItemConfig, index: number, selected: boolean) {
        return new PbcatItem(
            index,
            itemConfig.enumValue,
            itemConfig.title,
            this.calculateNextScreenName(itemConfig.nextScreenName),
            itemConfig.description,
            itemConfig.subHeading,
            itemConfig.imageUrls
                ? itemConfig.imageUrls
                : itemConfig.imageUrl
                    ? [itemConfig.imageUrl]
                    : [],
            selected);
    }

    private calculateNextScreenName(screenName: string | { [crashLocation: string]: string }): string {
        let nextScreenName: string;

        if (typeof screenName === 'string') {
            nextScreenName = screenName;
        }
        else if (screenName) {
            // determine which crash location was selected in step 1
            let crashLocation = this.stepHistory[0].selectedItem.enumValue;
            // get the corresponding next screen name (could be undefined, which indicates the flow is complete)
            nextScreenName = screenName[crashLocation];
        }

        return nextScreenName;
    }
}
