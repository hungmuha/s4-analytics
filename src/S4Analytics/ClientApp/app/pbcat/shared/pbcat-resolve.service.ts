import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import '../../rxjs-operators';
import { AppState } from '../../app.state';
import { PbcatService, PbcatInfoWithExists } from './pbcat.service';
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

    resolve(route: ActivatedRouteSnapshot): Observable<void> {
        let bikeOrPed = route.params['bikeOrPed'];
        let hsmvReportNumber = +route.params['hsmvReportNumber'];
        let stepNumber = +route.params['stepNumber'];
        let flowType = this.getFlowType(bikeOrPed);
        let config: PbcatConfig;
        return this.pbcatService
            .getConfiguration(flowType)
            .do(cfg => config = cfg)
            .switchMap(() => this.loadPbcatInfo(flowType, hsmvReportNumber))
            .switchMap(p => this.loadPbcatFlow(config, flowType, hsmvReportNumber, p.pbcatInfo, p.exists, stepNumber))
            .catch(this.handleError);
    }

    private loadPbcatInfo(flowType: FlowType, hsmvReportNumber: number): Observable<PbcatInfoWithExists> {
        let isSameFlow = this.state.flow && this.state.flow.hsmvReportNumber === hsmvReportNumber;
        return isSameFlow
            ? Observable.of<PbcatInfoWithExists>(new PbcatInfoWithExists(this.state.flow.pbcatInfo, this.state.flow.typingExists))
            : this.pbcatService.getPbcatInfo(flowType, hsmvReportNumber);
    }

    private loadPbcatFlow(
        config: PbcatConfig,
        flowType: FlowType,
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        exists: boolean,
        stepNumber: number): Observable<any>
    {
        // THIS METHOD IS WEIRD

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
            // flow has invalid state, so raise an error
            return Observable.throw('Flow has invalid state.');
        }
        else if (stepNumber) {
            // nothing more to do; return an empty observable
            return Observable.of(undefined);
        }
        else {
            // we're at the summary page, so get the crash type
            return this.pbcatService
                .calculateCrashType(flowType, this.state.flow.pbcatInfo)
                .do(crashType => this.state.flow.crashType = crashType);
        }
    }

    private getFlowType(bikeOrPed: string) {
        return bikeOrPed === 'ped'
            ? FlowType.Pedestrian
            : FlowType.Bicyclist;
    }

    private handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        let errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }
}
