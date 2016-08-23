import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';
import { PbcatService, PbcatFlow, PbcatItem } from './shared';

class NavLink {
    constructor(
        public title: string,
        public route: any[]) { }
}

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
        if (!this.flow.hasValidState) {
            // ERROR
            delete this.flow;
            this.pbcatService.clear();
            this.router.navigate(['pbcat', this.hsmvReportNumber, 'step', 1]);
        }
    }

    private toggleAutoAdvance(autoAdvance: boolean) {
        this.flow.autoAdvance = autoAdvance;
    }

    private ready(): boolean {
        return this.flow && this.flow.hasValidState;
    }

    private selectItem(pbcatItem: PbcatItem): void {
        this.flow.selectItemForCurrentStep(pbcatItem);
        if (this.flow.autoAdvance) {
            this.proceed();
        }
    }

    private showBackLink(): boolean {
        return this.flow.previousStep !== undefined;
    }

    private showProceedLink(): boolean {
        return (!this.flow.showSummary && this.flow.isFinalStep && this.flow.isFlowComplete) || this.flow.nextStep !== undefined;
    }

    private backLink(): NavLink {
        let navLink: NavLink;
        if (this.showBackLink()) {
            let previousStepNumber = this.stepNumber ? this.stepNumber - 1 : this.flow.stepHistory.length;
            navLink = new NavLink(
                this.flow.previousStep.title,
                ['/pbcat', this.hsmvReportNumber, 'step', previousStepNumber]);
        }
        return navLink;
    }

    private proceedLink(): NavLink {
        let navLink: NavLink;
        if (this.showProceedLink()) {
            if (this.flow.isFinalStep) {
                navLink = new NavLink(
                    "Summary",
                    ['/pbcat', this.hsmvReportNumber, 'summary']
                );
            }
            else {
                let nextStepNumber = this.stepNumber + 1;
                navLink = new NavLink(
                    this.flow.nextStep.title,
                    ['/pbcat', this.hsmvReportNumber, 'step', nextStepNumber]);
            }
        }
        return navLink;
    }

    private proceed(): void {
        let navLink = this.proceedLink();
        if (navLink !== undefined) {
            this.router.navigate(navLink.route);
        }
    }

    private saveAndClose(): void {
        this.flow.saveAndComplete();
    }

    private saveAndNext(): void {
        let hsmvReportNumber = this.flow.saveAndNext();
        this.router.navigate(['pbcat', hsmvReportNumber, 'step', 1]);
    }
}
