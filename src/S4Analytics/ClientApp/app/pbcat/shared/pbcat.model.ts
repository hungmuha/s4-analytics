import { PbcatStep } from './pbcat-step.model';
import { ParticipantType } from './pbcat.enums';

export class Pbcat {
    hsmvReportNumber: number;
    hsmvReportNumberDisplay: string;
    stepHistory: PbcatStep[];
    currentStep: PbcatStep;
    previousStep: PbcatStep;
    nextStep: PbcatStep;
    participantType: ParticipantType;
    autoAdvance: boolean;
    showReturnToSummary: boolean;
    flowComplete: boolean;

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
}
