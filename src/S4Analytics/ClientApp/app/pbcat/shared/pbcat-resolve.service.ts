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
export class PbcatResolveService implements Resolve<void> {
    private currentFlow: PbcatFlow;
    private isSameFlow: boolean;

    constructor(
        private pbcatService: PbcatService,
        private router: Router,
        private appState: AppState) { }

    resolve(route: ActivatedRouteSnapshot): Observable<PbcatFlow> {
        let hsmvReportNumber = +route.params['hsmvReportNumber'];
        let stepNumber = +route.params['stepNumber'];
        let flowType = FlowType.Pedestrian; // todo: set dynamically
        let config: PbcatConfig;
        this.currentFlow = this.appState.pbcatState.flow;
        this.isSameFlow = this.currentFlow && this.currentFlow.hsmvReportNumber === hsmvReportNumber;
        if (this.isSameFlow) {
            return this.goToStepOrSummary(this.currentFlow, stepNumber)
                .catch(this.handleError);
        }
        else {
            return this.pbcatService.getConfiguration(flowType)
                .do(cfg => config = cfg)
                .switchMap(() => this.pbcatService.getPbcatInfo(flowType, hsmvReportNumber))
                .map(p => new PbcatFlow(flowType, hsmvReportNumber, p.exists, p.pbcatInfo, config))
                .switchMap(flow => this.goToStepOrSummary(flow, stepNumber))
                .catch(this.handleError);
        }
    }

    private goToStepOrSummary(flow: PbcatFlow, stepNumber?: number): Observable<PbcatFlow> {
        if (stepNumber) {
            flow.goToStep(stepNumber);
        }
        else {
            flow.goToSummary();
        }

        if (flow.hasValidState) {
            return Observable.of<PbcatFlow>(flow);
        }
        else {
            // todo: show some kind of 404 page
            return this.handleError('404 Not Found');
        }
    }

    private handleError(error: any) {
        // In a real world app, we might use a remote logging infrastructure
        // We'd also dig deeper into the error to get a better message
        let errMsg = (error.message)
            ? error.message
            : error.status
                ? `${error.status} - ${error.statusText}`
                : error;
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }
}
