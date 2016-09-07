import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { AppState } from '../../app.state';
import { PbcatService } from './pbcat.service';
import { PbcatState } from './pbcat.state';
import { PbcatFlow, FlowType } from './pbcat-flow';
import { PbcatInfo } from './pbcat-info';
import { PbcatConfig } from './pbcat-config';

@Injectable()
export class PbcatResolveService implements Resolve<PbcatState> {
    private state: PbcatState;

    constructor(
        private pbcatService: PbcatService,
        private router: Router,
        private appState: AppState) {
        this.state = this.appState.pbcatState;
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
            .catch(() => this.onError());
    }

    private loadPbcatInfo(flowType: FlowType, hsmvReportNumber: number): Promise<[PbcatInfo, boolean]> {
        let isSameFlow = this.state.flow && this.state.flow.hsmvReportNumber === hsmvReportNumber;
        return isSameFlow
            ? Promise.resolve([this.state.flow.pbcatInfo, this.state.flow.typingExists])
            : this.pbcatService.getPbcatInfo(flowType, hsmvReportNumber);
    }

    private loadPbcatFlow(
        config: PbcatConfig,
        flowType: FlowType,
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        exists: boolean,
        stepNumber: number): Promise<any>
    {
        let isSameFlow = this.state.flow && this.state.flow.hsmvReportNumber === hsmvReportNumber;
        if (!isSameFlow) {
            this.state.flow = new PbcatFlow(flowType, hsmvReportNumber, exists, pbcatInfo, config);
        }
        if (stepNumber) {
            this.state.flow.goToStep(stepNumber);
        }
        else {
            this.state.flow.goToSummary();
        }
        if (!this.state.flow.hasValidState) {
            // todo: do something
        }
        if (!stepNumber) {
            return this.pbcatService
                .calculateCrashType(flowType, this.state.flow.pbcatInfo)
                .then(crashType => this.state.flow.crashType = crashType);
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

    private onError() {
        // todo: do something
    }
}
