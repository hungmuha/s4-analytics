import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { PbcatService, PbcatFlow, PbcatItem } from './shared';

@Component({
    selector: 'pbcat-master',
    template: require('./pbcat-master.component.html')
})
export class PbcatMasterComponent {
    private paramSub: Subscription;
    private hsmvReportNumber: number;
    private stepNumber: number;
    private flow: PbcatFlow;

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private pbcatService: PbcatService) { }

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

    private processParams(params: {[key: string]: string}): void {
        this.hsmvReportNumber = +params['hsmvReportNumber'];
        this.stepNumber = +params['stepNumber'];
        this.pbcatService.configure(
            () => this.getPbcatFlow()
        );
    }

    private getPbcatFlow(): void {
        this.flow = this.stepNumber
            ? this.pbcatService.getPbcatFlowAtStep(this.hsmvReportNumber, this.stepNumber)
            : this.pbcatService.getPbcatFlowAtSummary(this.hsmvReportNumber);
    }

    ready(): boolean {
        return this.flow && this.flow.hasValidState;
    }

    routeError(): boolean {
        return this.flow && !this.flow.hasValidState;
    }

    selectItem(pbcatItem: PbcatItem): void {
        this.flow.selectItemForCurrentStep(pbcatItem);
        this.proceed();
    }

    back(): void {
        let previousStepNumber = this.stepNumber - 1;
        this.router.navigate(['pbcat', this.hsmvReportNumber, 'step', previousStepNumber]);
    }

    proceed(): void {
        let nextStepNumber = this.stepNumber + 1;
        if (nextStepNumber <= 11) {
            this.router.navigate(['pbcat', this.hsmvReportNumber, 'step', nextStepNumber]);
        }
        else {
            this.goToSummary();
        }
    }

    goToSummary(): void {
        this.router.navigate(['pbcat', this.hsmvReportNumber, 'summary']);
    }

    saveAndClose(): void {
        this.flow.saveAndComplete();
    }

    saveAndNext(): void {
        let hsmvReportNumber = this.flow.saveAndNext();
        this.router.navigate(['pbcat', hsmvReportNumber, 'step', 1]);
    }
}
