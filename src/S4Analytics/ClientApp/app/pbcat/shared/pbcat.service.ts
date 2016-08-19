import { Injectable } from '@angular/core';
import { PbcatFlow } from './pbcat-flow.model';

@Injectable()
export class PbcatService {
    private flow: PbcatFlow;

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
            ? new PbcatFlow(hsmvReportNumber)
            : this.flow;
        return flow;
    }
}
