import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AppState } from '../app.state';
import {
    PbcatService, PbcatItem,
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
        return this.state.flowType === FlowType.Pedestrian
            ? 'Pedestrian Crash Typing'
            : 'Bicyclist Crash Typing';
    }

    private get ready(): boolean { return this.state && this.state.hasValidState; }

    private get autoAdvance() { return this.state.autoAdvance; }

    private set autoAdvance(value: boolean) { this.state.autoAdvance = value; }

    private get hsmvReportNumber() { return this.state.hsmvReportNumber; }

    private get showSummary() { return this.state.showSummary; }

    private get currentStepNumber() { return this.state.currentStepNumber; }

    private get currentStep() { return this.state.currentStep; }

    private get stepHistory() { return this.state.stepHistory; }

    private get crashType() { return this.state.crashType; }

    private selectItem(pbcatItem: PbcatItem): void {
        this.state.selectItemForCurrentStep(pbcatItem);
        if (this.autoAdvance) {
            // a 300ms delay to give visual confirmation of selected item
            setTimeout(() => this.proceed(), 300);
        }
    }

    private get showBackLink(): boolean {
        return this.state.canGoBack;
    }

    private get showProceedLink(): boolean {
        return this.state.canProceed;
    }

    private get showSummaryLink(): boolean {
        return this.state.canReturnToSummary;
    }

    private get backLinkText(): string {
        return `${this.state.previousStepNumber}. ${this.state.previousStep.title}`;
    }

    private get proceedLinkText(): string {
        return this.state.isFinalStep
            ? 'Summary'
            : `${this.state.nextStepNumber}. ${this.state.nextStep.title}`;
    }

    private get backLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flowType);
        return ['/pbcat', bikeOrPed, this.state.hsmvReportNumber, 'step', this.state.previousStepNumber];
    }

    private get proceedLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flowType);
        return this.state.isFinalStep
            ? ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary']
            : ['/pbcat', bikeOrPed, this.state.hsmvReportNumber, 'step', this.state.nextStepNumber];
    }

    private get summaryRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flowType);
        return ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary'];
    }

    private proceed(): void {
        if (this.proceedLinkRoute) {
            this.router.navigate(this.proceedLinkRoute);
        }
    }

    private jumpBackToStep(stepNumber: number) {
        let bikeOrPed = this.getBikeOrPed(this.state.flowType);
        let route = ['/pbcat', bikeOrPed, this.state.hsmvReportNumber, 'step', stepNumber];
        this.router.navigate(route);
    }

    private saveAndClose(): void {
        this.pbcatService.savePbcatInfo(
            this.state.flowType,
            this.state.hsmvReportNumber,
            this.state.pbcatInfo);
    }

    private saveAndNext(): void {
        let nextHsmvNumber: number;
        let flowType: FlowType;
        this.pbcatService
            .savePbcatInfo(
                this.state.flowType,
                this.hsmvReportNumber,
                this.state.pbcatInfo,
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
