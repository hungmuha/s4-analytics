import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { AppState } from '../../app.state';
import { PbcatState } from './pbcat.state';
import { PbcatService } from './pbcat.service';
import { PbcatFlow, FlowType } from './pbcat-flow';

@Injectable()
export class PbcatResolve implements Resolve<PbcatFlow> {
    private state: PbcatState;

    constructor(
        private pbcatService: PbcatService,
        private router: Router,
        private appState: AppState) {
        this.state = appState.pbcatState;
    }

    resolve(route: ActivatedRouteSnapshot): Promise<void> {
        let bikeOrPed = route.params['bikeOrPed'];
        let hsmvReportNumber = +route.params['hsmvReportNumber'];
        let stepNumber = +route.params['stepNumber'];
        let flowType = this.getFlowType(bikeOrPed);
        return this.pbcatService
            .getConfiguration(flowType)
            .then(config => this.state.config = config)
            .then(() => this.loadPbcatFlow(flowType, hsmvReportNumber, stepNumber))
            .catch(this.notFound);
    }

    private getFlowType(bikeOrPed: string) {
        return bikeOrPed === 'ped'
            ? FlowType.Pedestrian
            : FlowType.Bicyclist;
    }

    private loadPbcatFlow(flowType: FlowType, hsmvReportNumber: number, stepNumber: number): void {
        let isSameFlow = this.state.flow && this.state.flow.hsmvReportNumber === hsmvReportNumber;
        // migrate the auto-advance setting to the new flow
        let autoAdvance = this.state.flow ? this.state.flow.autoAdvance : true;
        this.state.flow = isSameFlow
            ? this.state.flow
            : new PbcatFlow(this.state.config, flowType, hsmvReportNumber, autoAdvance);
        if (stepNumber) {
            this.state.flow.goToStep(stepNumber);
        }
        else {
            this.state.flow.goToSummary();
        }
        if (!this.state.flow.hasValidState) {
            this.notFound();
        }
        if (!stepNumber) {
            this.pbcatService
                .calculateCrashType(this.state.flow.pbcatInfo)
                .then(crashType => this.state.crashType = crashType);
        }
    }

    private notFound() {
        // fake 404 page
        this.router.navigate(['404']);
    }
}
