import { PbcatInfo, PbcatPedestrianInfo, PbcatBicyclistInfo } from './pbcat-info';
import { PbcatStep } from './pbcat-step';
import { PbcatItem } from './pbcat-item';
import { PbcatCrashType } from './pbcat-crash-type';
import { PbcatConfig, PbcatScreenConfig, PbcatItemConfig } from './pbcat-config.d';
import { PbcatParticipantInfo } from './pbcat.service';

export enum FlowType {
    Undetermined,
    Bicyclist,
    Pedestrian
}

const PED_INITIAL_SCREEN: string = 'P-1';
const BIKE_INITIAL_SCREEN: string = 'B-1';
const UNDETERMINED_INITIAL_SCREEN: string = 'PB';

export class PbcatFlow {
    crashType: PbcatCrashType;
    isSaved: boolean = false;
    flowType: FlowType = FlowType.Undetermined;
    private originalFlowType: FlowType = FlowType.Undetermined;
    private _currentStepIndex: number = -1;
    private _stepHistory: PbcatStep[] = [];
    private _isFlowComplete: boolean = false;
    private crashLocAttrValue: string;
    private notes: string;

    constructor(
        public hsmvReportNumber: number,
        participantInfo: PbcatParticipantInfo,
        pbcatInfo: PbcatInfo,
        private config: PbcatConfig) {
        if (participantInfo.hasPedestrianTyping) {
            this.originalFlowType = FlowType.Pedestrian;
        }
        else if (participantInfo.hasBicyclistTyping) {
            this.originalFlowType = FlowType.Bicyclist;
        }
        this.flowType = this.determineFlowType(participantInfo);
        if (pbcatInfo !== undefined) {
            this.reconstructStepHistory(participantInfo, pbcatInfo);
        }
    }

    get typingExists(): boolean {
        // Because typing info is stored separately for bike or ped, we only return true
        // if the flow type is the same as before (bike and bike, for example).
        // This property ultimately determines whether we issue a create or an update call to the server.
        return this.flowType !== FlowType.Undetermined && this.flowType === this.originalFlowType;
    }

    get stepHistory() { return this._stepHistory; }

    get isFlowComplete() { return this._isFlowComplete; }

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
        let info: PbcatInfo;

        if (this.flowType === FlowType.Pedestrian) {
            info = new PbcatPedestrianInfo();
        }
        else if (this.flowType === FlowType.Bicyclist) {
            info = new PbcatBicyclistInfo();
        }

        if (info) {
            info.notes = this.notes;
            for (let step of this._stepHistory) {
                if (step.selectedItem !== undefined) {
                    (info as any)[step.infoAttrName] = step.selectedItem.infoAttrValue;
                }
            }
        }
        return info;
    }

    goToStep(stepNumber: number) {
        let stepIndex = stepNumber - 1; // stepNumber is 1-based
        let isNewFlow = stepIndex === 0 && this._stepHistory.length === 0;
        this._currentStepIndex = stepIndex;

        if (isNewFlow) {
            let currentStep = this.getFirstStep();
            this._stepHistory.push(currentStep);
        }
    }

    goToSummary() {
        if (this._isFlowComplete) {
            this._currentStepIndex = this._stepHistory.length; // summary isn't actually in the stepHistory
        }
    }

    selectItemForCurrentStep(item: PbcatItem) {
        let isCurrentSelectedItem = this.currentStep.selectedItem === item;

        // if flow type was undetermined, the first step will be the "bike or ped" selection,
        // so we need to set the flowType now that the user has made a decision.
        if (this.currentStep.screenName === UNDETERMINED_INITIAL_SCREEN) {
            if (item.infoAttrValue === 'Pedestrian') {
                this.flowType = FlowType.Pedestrian;
            }
            else if (item.infoAttrValue === 'Bicyclist') {
                this.flowType = FlowType.Bicyclist;
            }
        }
        else if (this.currentStep.screenName === PED_INITIAL_SCREEN || this.currentStep.screenName === BIKE_INITIAL_SCREEN) {
            this.crashLocAttrValue = item.infoAttrValue;
        }

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

    private determineFlowType(participantInfo: PbcatParticipantInfo): FlowType {
        if (participantInfo.hasPedestrianParticipant && !participantInfo.hasBicyclistParticipant) {
            return FlowType.Pedestrian;
        }
        else if (participantInfo.hasBicyclistParticipant && !participantInfo.hasPedestrianParticipant) {
            return FlowType.Bicyclist;
        }
        return FlowType.Undetermined;
    }

    private reconstructStepHistory(participantInfo: PbcatParticipantInfo, pbcatInfo: PbcatInfo) {
        // replay the previous session
        while (!this._isFlowComplete) {
            this.goToStep(this.currentStepNumber + 1);
            if (this.currentStep === undefined) {
                // this can occur if the previous session was not completed/imported properly
                break;
            }
            for (let item of this.currentStep.items) {
                // if the flow type is undetermined, we can determine it by which typing flow was used previously
                if (this.currentStep.screenName === UNDETERMINED_INITIAL_SCREEN) {
                    let pedTypingMatch = participantInfo.hasPedestrianTyping && item.infoAttrValue === 'Pedestrian';
                    let bikeTypingMatch = participantInfo.hasBicyclistTyping && item.infoAttrValue === 'Bicyclist';
                    if (pedTypingMatch || bikeTypingMatch) {
                        this.selectItemForCurrentStep(item);
                        break;
                    }
                }
                else if (item.infoAttrValue === (pbcatInfo as any)[this.currentStep.infoAttrName]) {
                    this.selectItemForCurrentStep(item);
                    break;
                }
            }
        }
        // grab the notes
        this.notes = pbcatInfo.notes;
        // then return to step 1
        this.goToStep(1);
    }

    private getFirstStep(): PbcatStep {
        // if the flow type could not be determined from the participant info,
        // the user must decide which to use. hence we show a special screen for this.
        // otherwise, we enter the bike or ped flow automatically.
        let screenName: string;
        switch (this.flowType) {
            case FlowType.Pedestrian:
                screenName = PED_INITIAL_SCREEN;
                break;
            case FlowType.Bicyclist:
                screenName = BIKE_INITIAL_SCREEN;
                break;
            default:
                screenName = UNDETERMINED_INITIAL_SCREEN;
                break;
        }
        let screenConfig = this.config[screenName];
        let step = new PbcatStep(screenName, screenConfig.title, screenConfig.description, screenConfig.infoAttrName);
        step.items = screenConfig.items.map((item, index) => this.itemFromItemConfig(item, index));
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
            step = new PbcatStep(screenName, screenConfig.title, screenConfig.description, screenConfig.infoAttrName);
            step.items = screenConfig.items.map((item, index) => this.itemFromItemConfig(item, index));
        }

        return step;
    }

    private itemFromItemConfig(itemConfig: PbcatItemConfig, index: number) {
        return new PbcatItem(
            index,
            itemConfig.infoAttrValue,
            itemConfig.title,
            this.calculateNextScreenName(itemConfig.nextScreenName),
            itemConfig.description,
            itemConfig.subHeading,
            itemConfig.imageUrls
                ? itemConfig.imageUrls
                : itemConfig.imageUrl
                    ? [itemConfig.imageUrl]
                    : []);
    }

    private calculateNextScreenName(screenName: string | { [crashLocation: string]: string }): string {
        let nextScreenName: string;

        if (typeof screenName === 'string') {
            // In most cases, the config JSON looks like this example:
            //   "nextScreenName": "P-1A"
            // There is only one possibility.
            nextScreenName = screenName;
        }
        else if (screenName) {
            // In a few cases, the config JSON looks like this example:
            //  "nextScreenName": {
            //    "Intersection": "B-3",
            //    "IntersectionRelated": "B-3",
            //    "NonIntersectionLocation": "B-3"
            //  }
            // To determine the next screen name, we consult the crash location that was
            // captured on the first step of the flow. It's possible that the result is undefined,
            // if the crash location is not one of the keys in nextScreenName.
            nextScreenName = screenName[this.crashLocAttrValue];
        }

        return nextScreenName; // If undefined, that's OK ... it means we have reached the end of the flow.
    }
}
