import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { AppState } from '../app.state.ts';
import {
    PbcatService, PbcatFlow, PbcatItem,
    PbcatCrashType, ParticipantType, PbcatState
} from './shared';

@Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    private paramSub: Subscription;
    private state: PbcatState;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private appState: AppState,
        private pbcatService: PbcatService) {
        this.state = appState.pbcatState;
    }

    ngOnInit(): void {
        // subscribe to params
        this.paramSub = this.activatedRoute.params.subscribe(
            params => this.processParams(params)
        );
    }

    ngOnDestroy(): void {
        // unsubscribe from params
        this.paramSub.unsubscribe();
    }

    private processParams(params: any): void {
        let bikeOrPed = params['bikeOrPed'];
        let hsmvReportNumber = +params['hsmvReportNumber'];
        let stepNumber = +params['stepNumber'];
        let participantType = this.getParticipantType(bikeOrPed);
        this.pbcatService
            .getConfiguration(participantType)
            .then(config => this.state.config = config)
            .then(() => this.getPbcatFlow(participantType, hsmvReportNumber, stepNumber));
    }

    private getPbcatFlow(participantType: ParticipantType, hsmvReportNumber: number, stepNumber: number): void {
        let isSameFlow = this.state.flow && this.state.flow.hsmvReportNumber === hsmvReportNumber;
        // migrate the auto-advance setting to the new flow
        let autoAdvance = this.state.flow ? this.state.flow.autoAdvance : true;
        this.state.flow = isSameFlow
            ? this.state.flow
            : new PbcatFlow(this.state.config, participantType, hsmvReportNumber, autoAdvance);
        if (stepNumber) {
            this.state.flow.goToStep(stepNumber);
        }
        else {
            this.state.flow.goToSummary();
        }
        if (!this.state.flow.hasValidState) {
            this.notFound(hsmvReportNumber);
        }
        if (!stepNumber) {
            this.pbcatService
                .calculateCrashType(this.state.flow.pbcatInfo)
                .then(crashType => this.state.crashType = crashType);
        }
    }

    private notFound(hsmvReportNumber: number) {
        // fake 404 page
        this.router.navigate(['404']);
    }

    private get pageTitle(): string {
        return this.state.flow.participantType === ParticipantType.Pedestrian
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
        let bikeOrPed = this.getBikeOrPed(this.state.flow.participantType);
        return ['/pbcat', bikeOrPed, this.state.flow.hsmvReportNumber, 'step', this.state.flow.previousStepNumber];
    }

    private get proceedLinkRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.participantType);
        return this.state.flow.isFinalStep
            ? ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary']
            : ['/pbcat', bikeOrPed, this.state.flow.hsmvReportNumber, 'step', this.state.flow.nextStepNumber];
    }

    private get summaryRoute(): any[] {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.participantType);
        return ['/pbcat', bikeOrPed, this.hsmvReportNumber, 'summary'];
    }

    private proceed(): void {
        if (this.proceedLinkRoute) {
            this.router.navigate(this.proceedLinkRoute);
        }
    }

    private jumpBackToStep(stepNumber: number) {
        let bikeOrPed = this.getBikeOrPed(this.state.flow.participantType);
        let route = ['/pbcat', bikeOrPed, this.state.flow.hsmvReportNumber, 'step', stepNumber];
        this.router.navigate(route);
    }

    private saveAndClose(): void {
        this.pbcatService.savePbcatInfo(
            this.state.flow.participantType,
            this.state.flow.hsmvReportNumber,
            this.state.flow.pbcatInfo);
    }

    private saveAndNext(): void {
        let nextHsmvNumber: number;
        let participantType: ParticipantType;
        this.pbcatService
            .savePbcatInfo(
                this.state.flow.participantType,
                this.hsmvReportNumber,
                this.state.flow.pbcatInfo,
                true)
            .then(([participantType, nextHsmvNumber]) =>
                this.router.navigate(['/pbcat', this.getBikeOrPed(participantType), nextHsmvNumber, 'step', 1]));
    }

    private getBikeOrPed(participantType: ParticipantType) {
        return participantType === ParticipantType.Pedestrian
            ? 'ped'
            : 'bike';
    }

    private getParticipantType(bikeOrPed: string) {
        return bikeOrPed === 'ped'
            ? ParticipantType.Pedestrian
            : ParticipantType.Bicyclist;
    }
}
