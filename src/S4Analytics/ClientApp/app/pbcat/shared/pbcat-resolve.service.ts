import { Injectable } from '@angular/core';
import { Router, Resolve, ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import '../../rxjs-operators';
import { AppState } from '../../app.state';
import { PbcatService, PbcatParticipantInfo } from './pbcat.service';
import { PbcatState } from './pbcat.state';
import { PbcatFlow } from './pbcat-flow';
import { PbcatInfo } from './pbcat-info';
import { PbcatConfig } from './pbcat-config';

@Injectable()
export class PbcatResolveService implements Resolve<PbcatFlow> {
    private currentFlow: PbcatFlow;
    private isSameFlow: boolean;

    constructor(
        private pbcatService: PbcatService,
        private router: Router,
        private appState: AppState) { }

    resolve(route: ActivatedRouteSnapshot): Observable<any> {
        let hsmvReportNumber = +route.params['hsmvReportNumber'];
        let stepNumber = +route.params['stepNumber'];
        let token = route.params['token'];
        this.currentFlow = this.appState.pbcatState.flow;
        this.isSameFlow = this.currentFlow && this.currentFlow.hsmvReportNumber === hsmvReportNumber;
        if (this.isSameFlow) {
            return Observable.of(this.currentFlow)
                .do(flow => this.goToStepOrSummary(flow, stepNumber));
        }
        else {
            // if this is a new flow, it must start at step 1
            if (stepNumber === 1) {
                /*
                1. get pbcat config
                2. get participant info
                3. config that crash has bike or ped
                4. get existing typing info
                5. create flow
                6. advance to step 1
                */
                let config: PbcatConfig;
                let participantInfo: PbcatParticipantInfo;
                let retVal: Observable<any> = this.pbcatService.getConfiguration()
                    .do(cfg => config = cfg);
                if (token) {
                    retVal = retVal
                        .switchMap(() => this.pbcatService.getSession(token))
                        .do(queue => this.appState.pbcatState.queue = queue);
                }
                retVal = retVal
                    .switchMap(() => this.pbcatService.getParticipantInfo(hsmvReportNumber))
                    .switchMap(info => this.confirmBikePedCrash(info)) // sanity check
                    .do(info => participantInfo = info)
                    .switchMap(() => this.pbcatService.getPbcatInfo(participantInfo, hsmvReportNumber))
                    .map(pbcatInfo => new PbcatFlow(hsmvReportNumber, participantInfo, pbcatInfo, config))
                    .do(flow => this.goToStepOrSummary(flow, 1))
                    .catch(this.handleError);
                return retVal;
            }
            else {
                this.router.navigate(['pbcat', hsmvReportNumber, 'step', 1]);
            }
        }
    }

    private confirmBikePedCrash(participantInfo: PbcatParticipantInfo): Observable<any> {
        if (participantInfo.hasBicyclistParticipant || participantInfo.hasPedestrianParticipant) {
            return Observable.of(participantInfo);
        }
        else {
            return Observable.throw('The crash has no pedestrian or bicyclist participants and cannot be typed.');
        }
    }

    private goToStepOrSummary(flow: PbcatFlow, stepNumber?: number) {
        if (stepNumber) {
            flow.goToStep(stepNumber);
        }
        else {
            flow.goToSummary();
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
