import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { AppState } from '../../app.state';
import { PbcatService } from './pbcat.service';
import { PbcatState, FlowType } from './pbcat.state';
import { PbcatConfig } from './pbcat-config';

@Injectable()
export class PbcatResolveService implements Resolve<PbcatState> {
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
            .then(config => this.loadPbcatFlow(config, flowType, hsmvReportNumber, stepNumber))
            .catch(this.notFound);
    }

    private loadPbcatFlow(config: PbcatConfig, flowType: FlowType, hsmvReportNumber: number, stepNumber: number): void {
        let isSameFlow = this.state && this.state.hsmvReportNumber === hsmvReportNumber;
        // migrate the auto-advance setting to the new flow
        let autoAdvance = this.state ? this.state.autoAdvance : true;
        if (!isSameFlow) {
            this.state.resetFlow(config, flowType, hsmvReportNumber, autoAdvance);
        }
        if (stepNumber) {
            this.state.goToStep(stepNumber);
        }
        else {
            this.state.goToSummary();
        }
        if (!this.state.hasValidState) {
            this.notFound();
        }
        if (!stepNumber) {
            this.pbcatService
                .calculateCrashType(this.state.pbcatInfo)
                .then(crashType => this.state.crashType = crashType);
        }
    }

    private getFlowType(bikeOrPed: string) {
        return bikeOrPed === 'ped'
            ? FlowType.Pedestrian
            : FlowType.Bicyclist;
    }

    private notFound() {
        // fake 404 page
        this.router.navigate(['404']);
    }
}
