import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { ParticipantType } from './pbcat.enums';
import { PbcatFlow } from './pbcat-flow.model';
import { PbcatConfig } from './pbcat-config.d.ts';
import { PbcatCrashType } from './pbcat-crash-type.model';
import { PbcatInfo, PbcatBicyclistInfo, PbcatPedestrianInfo } from './pbcat-info.model';

@Injectable()
export class PbcatService {
    private config: PbcatConfig;
    private flow: PbcatFlow;

    constructor(private http: Http) { }

    configure(participantType: ParticipantType, onConfigured: () => void) {
        let jsonUrl = participantType === ParticipantType.Pedestrian
            ? 'json/pbcat-ped.json'
            : 'json/pbcat-bike.json';
        if (this.config === undefined ||
            this.config.participantType !== participantType) {
            this.http
                .get(jsonUrl)
                .map(response => response.json())
                .subscribe(config => {
                    this.config = config;
                    this.config.participantType = participantType;
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

    getPbcatInfo(hsmvReportNumber: number): PbcatInfo {
        // GET /api/pbcat/:bikeOrPed/:hsmvRptNr
        // todo: reconstruct stepHistory for previously typed crashes
        return this.config.participantType === ParticipantType.Pedestrian
            ? new PbcatPedestrianInfo()
            : new PbcatBicyclistInfo();
    }

    savePbcatInfo(
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        getNextCrash: boolean = false): [ParticipantType, number] {
        // POST /api/pbcat/:bikeOrPed
        //  PUT /api/pbcat/:bikeOrPed/:hsmvRptNr
        // mock get the actual next hsmv report number
        if (getNextCrash) {
            return [this.config.participantType, getNextCrash ? hsmvReportNumber + 1 : undefined];
        }
    }

    deletePbcatInfo(hsmvReportNumber: number): void {
        //  DELETE /api/pbcat/:bikeOrPed/:hsmvRptNr
    }

    calculateCrashType(pbcatInfo: PbcatInfo): PbcatCrashType {
        // GET /api/pbcat/:bikeOrPed/crashtype
        // mock crash type
        let crashType = new PbcatCrashType();
        crashType.crashTypeNbr = 781;
        crashType.crashTypeDesc = 'Motorist Left Turn - Parallel Paths';
        crashType.crashGroupNbr = 790;
        crashType.crashGroupDesc = 'Crossing Roadway - Vehicle Turning';
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
