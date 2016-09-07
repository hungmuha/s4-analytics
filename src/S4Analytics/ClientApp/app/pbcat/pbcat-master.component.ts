import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AppState } from '../app.state';
import {
    PbcatService, PbcatItem, PbcatCrashType,
    PbcatFlow, FlowType, PbcatState
} from './shared';

@Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    private state: PbcatState;

    constructor(
        private router: Router,
        private appState: AppState,
        private pbcatService: PbcatService) {
        this.state = appState.pbcatState;
    }

    private get flow(): PbcatFlow { return this.state.flow; }

    private get pageTitle(): string {
        return this.flow.flowType === FlowType.Pedestrian
            ? 'Pedestrian Crash Typing'
            : 'Bicyclist Crash Typing';
    }

    private launchReportViewer() {
        if (this.state.reportViewerWindow && !this.state.reportViewerWindow.closed) {
            this.state.reportViewerWindow.location.href = `/pbcat/viewer/${this.hsmvReportNumber}`;
        }
        else {
            this.state.reportViewerWindow = window.open(`/pbcat/viewer/${this.hsmvReportNumber}`, 'crashReportWindow');
        }
    }

    private closeReportViewer() {
        if (this.state.reportViewerWindow && this.state.reportViewerWindow.close) {
            this.state.reportViewerWindow.close();
        }
        this.state.reportViewerWindow = undefined;
    }

    private get isSaved(): boolean { return this.flow.isSaved; }

    private get hsmvReportNumber() { return this.flow.hsmvReportNumber; }

    private get showSummary() { return this.flow.showSummary; }

    private get currentStepNumber() { return this.flow.currentStepNumber; }

    private get currentStep() { return this.flow.currentStep; }

    private get stepHistory() { return this.flow.stepHistory; }

    private get crashType() { return this.flow.crashType; }

    private get canGoBack(): boolean { return this.flow.canGoBack; }

    private get canProceed(): boolean { return this.flow.canProceed; }

    private get canReturnToSummary(): boolean { return this.flow.canReturnToSummary; }

    private get ready(): boolean {
        return this.flow && this.flow.hasValidState;
    }

    private get nextCrashExists(): boolean { return this.flow.isSaved && this.state.nextHsmvNumber !== undefined; }

    private get autoAdvance() { return this.state.autoAdvance; }

    private set autoAdvance(value: boolean) { this.state.autoAdvance = value; }

    private get showReportViewer() { return this.state.showReportViewer; }

    private set showReportViewer(value: boolean) {
        if (this.state.showReportViewer !== value) {
            this.state.showReportViewer = value;
            if (value) {
                this.launchReportViewer();
            }
            else {
                this.closeReportViewer();
            }
        }
    }

    private selectItem(pbcatItem: PbcatItem): void {
        this.flow.selectItemForCurrentStep(pbcatItem);
        if (this.autoAdvance) {
            // a 300ms delay to give visual confirmation of selected item
            setTimeout(() => this.proceed(), 300);
        }
    }

    private get backLinkText(): string {
        return `${this.flow.previousStepNumber}. ${this.flow.previousStep.title}`;
    }

    private get proceedLinkText(): string {
        return this.flow.isFinalStep
            ? 'Summary'
            : `${this.flow.nextStepNumber}. ${this.flow.nextStep.title}`;
    }

    private get backLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.flow.flowType);
        return ['/pbcat', bikeOrPed, this.flow.hsmvReportNumber, 'step', this.flow.previousStepNumber];
    }

    private get proceedLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.flow.flowType);
        return this.flow.isFinalStep
            ? ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary']
            : ['/pbcat', bikeOrPed, this.flow.hsmvReportNumber, 'step', this.flow.nextStepNumber];
    }

    private get summaryRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.flow.flowType);
        return ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary'];
    }

    private proceed(): void {
        if (this.proceedLinkRoute) {
            this.router.navigate(this.proceedLinkRoute);
        }
    }

    private jumpBackToStep(stepNumber: number) {
        let bikeOrPed = this.getBikeOrPed(this.flow.flowType);
        let route = ['/pbcat', bikeOrPed, this.flow.hsmvReportNumber, 'step', stepNumber];
        this.router.navigate(route);
    }

    private handleSaved(flowType: FlowType, nextHsmvNumber: number) {
        this.flow.isSaved = true;
        this.state.nextHsmvNumber = nextHsmvNumber;
    }

    private get advanceToNextRoute(): any[] {
        return ['/pbcat', this.getBikeOrPed(this.state.nextFlowType), this.state.nextHsmvNumber, 'step', 1];
    }

    private acceptAndSave(): void {
        if (this.flow.typingExists) {
            this.pbcatService.updatePbcatInfo(this.flow)
                .then(([flowType, nextHsmvNumber]) => this.handleSaved(flowType, nextHsmvNumber));
        }
        else {
            this.pbcatService.createPbcatInfo(this.flow)
                .then(([flowType, nextHsmvNumber]) => this.handleSaved(flowType, nextHsmvNumber));
        }
    }

    private getBikeOrPed(flowType: FlowType) {
        return flowType === FlowType.Pedestrian
            ? 'ped'
            : 'bike';
    }
}
