import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AppState } from '../app.state';
import {
    PbcatService, PbcatFlow, PbcatItem,
    PbcatCrashType, FlowType, PbcatState
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

    private get pageTitle(): string {
        return this.state.flow.flowType === FlowType.Pedestrian
            ? 'Pedestrian Crash Typing'
            : 'Bicyclist Crash Typing';
    }

    private get ready(): boolean { return this.state.flow && this.state.flow.hasValidState; }

    private get autoAdvance() { return this.state.flow.autoAdvance; }

    private set autoAdvance(value: boolean) { this.state.flow.autoAdvance = value; }

    private get hsmvReportNumber() { return this.state.flow.hsmvReportNumber; }

    private get showSummary() { return this.state.flow.showSummary; }

    private get currentStepNumber() { return this.state.flow.currentStepNumber; }

    private get currentStep() { return this.state.flow.currentStep; }

    private get stepHistory() { return this.state.flow.stepHistory; }

    private get crashType() { return this.state.crashType; }

    private selectItem(pbcatItem: PbcatItem): void {
        this.state.flow.selectItemForCurrentStep(pbcatItem);
        if (this.autoAdvance) {
            // a 300ms delay to give visual confirmation of selected item
            setTimeout(() => this.proceed(), 300);
        }
    }

    private get showBackLink(): boolean {
        return this.state.flow.canGoBack;
    }

    private get showProceedLink(): boolean {
        return this.state.flow.canProceed;
    }

    private get showSummaryLink(): boolean {
        return this.state.flow.canReturnToSummary;
    }

    private get backLinkText(): string {
        return `${this.state.flow.previousStepNumber}. ${this.state.flow.previousStep.title}`;
    }

    private get proceedLinkText(): string {
        return this.state.flow.isFinalStep
            ? 'Summary'
            : `${this.state.flow.nextStepNumber}. ${this.state.flow.nextStep.title}`;
    }

    private get backLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.flowType);
        return ['/pbcat', bikeOrPed, this.state.flow.hsmvReportNumber, 'step', this.state.flow.previousStepNumber];
    }

    private get proceedLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.flowType);
        return this.state.flow.isFinalStep
            ? ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary']
            : ['/pbcat', bikeOrPed, this.state.flow.hsmvReportNumber, 'step', this.state.flow.nextStepNumber];
    }

    private get summaryRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.flowType);
        return ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary'];
    }

    private proceed(): void {
        if (this.proceedLinkRoute) {
            this.router.navigate(this.proceedLinkRoute);
        }
    }

    private jumpBackToStep(stepNumber: number) {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.flowType);
        let route = ['/pbcat', bikeOrPed, this.state.flow.hsmvReportNumber, 'step', stepNumber];
        this.router.navigate(route);
    }

    private saveAndClose(): void {
        this.pbcatService.savePbcatInfo(
            this.state.flow.flowType,
            this.state.flow.hsmvReportNumber,
            this.state.flow.pbcatInfo);
    }

    private saveAndNext(): void {
        let nextHsmvNumber: number;
        let flowType: FlowType;
        this.pbcatService
            .savePbcatInfo(
                this.state.flow.flowType,
                this.hsmvReportNumber,
                this.state.flow.pbcatInfo,
                true)
            .then(([partType, nextNum]) =>
                this.router.navigate(['/pbcat', this.getBikeOrPed(partType), nextNum, 'step', 1]));
    }

    private getBikeOrPed(flowType: FlowType) {
        return flowType === FlowType.Pedestrian
            ? 'ped'
            : 'bike';
    }
}
