import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { PbcatService, PbcatFlow, PbcatItem, PbcatCrashType, PbcatPedestrianInfo } from './shared';

@Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    private _paramSub: Subscription;
    private _flow: PbcatFlow;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

    ngOnInit(): void {
        // subscribe to params
        this._paramSub = this.activatedRoute.params.subscribe(
            params => this.processParams(params)
        );
    }

    ngOnDestroy(): void {
        // unsubscribe from params
        this._paramSub.unsubscribe();
    }

    private processParams(params: any): void {
        let hsmvReportNumber = +params['hsmvReportNumber'];
        let stepNumber = +params['stepNumber'];
        this.pbcatService.configure(
            () => this.getPbcatFlow(hsmvReportNumber, stepNumber)
        );
    }

    private getPbcatFlow(hsmvReportNumber: number, stepNumber: number): void {
        this._flow = stepNumber
            ? this.pbcatService.getPbcatFlowAtStep(hsmvReportNumber, stepNumber)
            : this.pbcatService.getPbcatFlowAtSummary(hsmvReportNumber);
        if (!this._flow.hasValidState) {
            // ERROR
            this.pbcatService.reset();
            this.router.navigate(['pbcat', hsmvReportNumber, 'step', 1]);
        }
    }

    private get ready(): boolean { return this._flow && this._flow.hasValidState; }

    private get autoAdvance() { return this._flow.autoAdvance; }

    private set autoAdvance(value: boolean) { this._flow.autoAdvance = value; }

    private get hsmvReportNumber() { return this._flow.hsmvReportNumber; }

    private get showSummary() { return this._flow.showSummary; }

    private get currentStepNumber() { return this._flow.currentStepNumber; }

    private get currentStep() { return this._flow.currentStep; }

    private get stepHistory() { return this._flow.stepHistory; }

    private selectItem(pbcatItem: PbcatItem): void {
        this._flow.selectItemForCurrentStep(pbcatItem);
        if (this.autoAdvance) {
            // a 300ms delay to give visual confirmation of selected item
            setTimeout(() => this.proceed(), 300);
        }
    }

    private get showBackLink(): boolean {
        return this._flow.canGoBack;
    }

    private get showProceedLink(): boolean {
        return this._flow.canProceed;
    }

    private get showSummaryLink(): boolean {
        return this._flow.canReturnToSummary;
    }

    private get backLinkText(): string {
        return `${this._flow.previousStepNumber}. ${this._flow.previousStep.title}`;
    }

    private get proceedLinkText(): string {
        return `${this._flow.nextStepNumber}. ${this._flow.nextStep.title}`;
    }

    private get backLinkRoute(): any[] {
        return ['/pbcat', this._flow.hsmvReportNumber, 'step', this._flow.previousStepNumber];
    }

    private get proceedLinkRoute(): any[] {
        return this._flow.isFinalStep
            ? ['/pbcat', this.hsmvReportNumber, 'summary']
            : ['/pbcat', this._flow.hsmvReportNumber, 'step', this._flow.nextStepNumber];
    }

    private get summaryRoute(): any[] {
        return ['/pbcat', this.hsmvReportNumber, 'summary'];
    }

    private proceed(): void {
        if (this.proceedLinkRoute) {
            this.router.navigate(this.proceedLinkRoute);
        }
    }

    private saveAndClose(): void {
        let pedInfo = this._flow.getPedInfo();
        this.pbcatService.savePbcatPedestrianInfo(this.hsmvReportNumber, pedInfo);
    }

    private saveAndNext(): void {
        let pedInfo = this._flow.getPedInfo();
        let nextHsmvNumber = this.pbcatService.savePbcatPedestrianInfo(this.hsmvReportNumber, pedInfo, true);
        this.router.navigate(['pbcat', nextHsmvNumber, 'step', 1]);
    }
}
