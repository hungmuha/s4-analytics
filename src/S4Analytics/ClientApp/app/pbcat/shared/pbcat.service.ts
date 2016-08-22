import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { PbcatFlow } from './pbcat-flow.model';
import { PbcatConfig } from './pbcat-config.d.ts';

@Injectable()
export class PbcatService {
    private config: PbcatConfig;
    private flow: PbcatFlow;

    constructor(private http: Http) { }

    configure(onConfigured: () => void) {
        if (this.config === undefined) {
            this.http
                .get('json/pbcat-ped.json')
                .map(response => response.json())
                .subscribe(config => {
                    this.config = config;
                    onConfigured();
                });
        }
        else {
            onConfigured();
        }
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
            ? new PbcatFlow(this.config, hsmvReportNumber)
            : this.flow;
        return flow;
    }
}
