import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { AppState } from '../../app.state';
import { PbcatService } from './pbcat.service';
import { PbcatState, FlowType } from './pbcat.state';
import { PbcatInfo } from './pbcat-info';
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
        let config: PbcatConfig;
        return this.pbcatService
            .getConfiguration(flowType)
            .then(c => config = c)
            .then(() => this.loadPbcatInfo(flowType, hsmvReportNumber))
            .then(([pbcatInfo, exists]) => this.loadPbcatFlow(config, flowType, hsmvReportNumber, pbcatInfo, exists, stepNumber))
            .catch(() => this.notFound());
    }

    private loadPbcatInfo(flowType: FlowType, hsmvReportNumber: number): Promise<[PbcatInfo, boolean]> {
        let isSameFlow = this.state && this.state.hsmvReportNumber === hsmvReportNumber;
        return isSameFlow
            ? Promise.resolve([this.state.pbcatInfo, this.state.exists])
            : this.pbcatService.getPbcatInfo(flowType, hsmvReportNumber);
    }

    private loadPbcatFlow(
        config: PbcatConfig,
        flowType: FlowType,
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        exists: boolean,
        stepNumber: number): Promise<any> {
        let isSameFlow = this.state && this.state.hsmvReportNumber === hsmvReportNumber;
        // migrate the auto-advance setting to the new flow
        let autoAdvance = this.state ? this.state.autoAdvance : true;
        let showReportViewer = this.state ? this.state.showReportViewer : false;
        if (!isSameFlow) {
            this.state.resetFlow(config, flowType, hsmvReportNumber, pbcatInfo, exists, autoAdvance, showReportViewer);
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
            return this.pbcatService
                .calculateCrashType(flowType, this.state.pbcatInfo)
                .then(crashType => this.state.crashType = crashType);
        }
        else {
            return Promise.resolve();
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
