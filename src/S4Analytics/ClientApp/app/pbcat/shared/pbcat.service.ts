import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { PbcatFlow } from './pbcat-flow.model';
import { PbcatConfig } from './pbcat-config.d.ts';
import { PbcatCrashType } from './pbcat-crash-type.model';
import { PbcatPedestrianInfo } from './pbcat-ped-info.model';

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

    reset() {
        delete this.flow;
    }

    getPbcatPedestrianInfo(hsmvReportNumber: number): PbcatPedestrianInfo {
        // GET /api/pbcat/ped/:hsmvRptNr
        // todo: reconstruct stepHistory for previously typed crashes
        return new PbcatPedestrianInfo();
    }

    savePbcatPedestrianInfo(
        hsmvReportNumber: number,
        pedInfo: PbcatPedestrianInfo,
        getNextCrash: boolean = false): number {
        // POST /api/pbcat/ped
        //  PUT /api/pbcat/ped/:hsmvRptNr
        // mock get the actual next hsmv report number
        return getNextCrash ? hsmvReportNumber + 1 : undefined;
    }

    deletePbcatPedestrianInfo(hsmvReportNumber: number): void {
        //  DELETE /api/pbcat/ped/:hsmvRptNr
    }

    calculatePedestrianCrashType(pedInfo: PbcatPedestrianInfo): PbcatCrashType {
        // GET /api/pbcat/ped/crashtype
        // mock crash type
        let crashType = new PbcatCrashType();
        crashType.crashTypeNbr = 781;
        crashType.crashTypeDesc = "Motorist Left Turn - Parallel Paths";
        crashType.crashGroupNbr = 790;
        crashType.crashGroupDesc = "Crossing Roadway - Vehicle Turning";
        crashType.crashTypeExpanded = 12781;
        crashType.crashGroupExpanded = 12790;
        return crashType;
    }

    getPbcatFlowAtSummary(hsmvReportNumber: number): PbcatFlow {
        this.flow = this.getPbcatFlow(hsmvReportNumber);
        this.flow.goToSummary();
        return this.flow;
    }

    getPbcatFlowAtStep(hsmvReportNumber: number, stepNumber: number): PbcatFlow {
        this.flow = this.getPbcatFlow(hsmvReportNumber);
        this.flow.goToStep(stepNumber);
        return this.flow;
    }

    private getPbcatFlow(hsmvReportNumber: number): PbcatFlow {
        let isSameFlow = this.flow && this.flow.hsmvReportNumber === hsmvReportNumber;
        // migrate the auto-advance setting to the new flow
        let autoAdvance = this.flow ? this.flow.autoAdvance : true;
        let flow = isSameFlow
            ? this.flow
            : new PbcatFlow(this.config, hsmvReportNumber, autoAdvance);
        return flow;
    }
}
