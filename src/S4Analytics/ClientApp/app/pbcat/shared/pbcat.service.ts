import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { PbcatFlow } from './pbcat-flow.model';

@Injectable()
export class PbcatService {
    private flow: PbcatFlow;
    private pbcatConfig: Observable<any>;

    constructor(private http: Http) {
        this.pbcatConfig = this.http.get('json/pbcat-ped.json')
            .map(response => response.json());
    }

    getPbcatFlowAtSummary(hsmvReportNumber: number): PbcatFlow {
        this.flow = this.getPbcatFlow(hsmvReportNumber);
        this.flow.goToSummary();
        return this.flow;
    }

    getPbcatFlowAtStep(hsmvReportNumber: number, stepNumber: number): PbcatFlow {
        this.flow = this.getPbcatFlow(hsmvReportNumber);
        this.flow.goToStepNumber(stepNumber);
        return this.flow;
    }

    private getPbcatFlow(hsmvReportNumber: number): PbcatFlow {
        let isNewFlow =
            this.flow === undefined ||
            this.flow.hsmvReportNumber !== hsmvReportNumber;
        let flow = isNewFlow
            ? new PbcatFlow(this.pbcatConfig, hsmvReportNumber)
            : this.flow;
        return flow;
    }
}
