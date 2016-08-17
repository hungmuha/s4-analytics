import { Injectable } from '@angular/core';
import { PbcatPedestrianInfo } from './pbcat-ped-info.model';
import { PbcatCrashType } from './pbcat-crash-type.model';
import { PbcatStep } from './pbcat-step.model';
import { PbcatItem } from './pbcat-item.model';

// todo: return promises

@Injectable()
export class PbcatService {
    getPbcatPedestrianInfo(hsmvReportNumber: number): PbcatPedestrianInfo {
        // GET /api/pbcat/ped/:hsmvRptNr
        return new PbcatPedestrianInfo();
    }

    savePbcatPedestrianInfo(
        hsmvReportNumber: number,
        pedInfo: PbcatPedestrianInfo,
        getNextCrash: boolean = false): number {
        // POST /api/pbcat/ped
        //  PUT /api/pbcat/ped/:hsmvRptNr
        return getNextCrash ? hsmvReportNumber + 1 : undefined;
    }

    deletePbcatPedestrianInfo(hsmvReportNumber: number): void {
        //  DELETE /api/pbcat/ped/:hsmvRptNr
    }

    calculatePedestrianCrashType(pedInfo: PbcatPedestrianInfo): PbcatCrashType {
        // GET /api/pbcat/ped/crashtype
        let crashType = new PbcatCrashType();
        crashType.crashTypeNbr = 781;
        crashType.crashTypeDesc = "Motorist Left Turn - Parallel Paths";
        crashType.crashGroupNbr = 790;
        crashType.crashGroupDesc = "Crossing Roadway - Vehicle Turning";
        crashType.crashTypeExpanded = 12781;
        crashType.crashGroupExpanded = 12790;
        return crashType;
    }

    getPedestrianNextStep(pedInfo: PbcatPedestrianInfo): PbcatStep {
        let step: PbcatStep;

        if (pedInfo.pedestrianPositionCd === undefined) {
            step = new PbcatStep('Where did the crash occur?', 'pedestrianPositionCd');
        }
        else if (pedInfo.motoristDirTravelCd === undefined) {
            step = new PbcatStep('What was the position of the pedestrian when struck?', 'motoristDirTravelCd');
        }
        else if (pedInfo.motoristManeuverCd === undefined) {
            step = new PbcatStep('Question 3', 'motoristManeuverCd');
        }
        else if (pedInfo.legOfIntrsectCd === undefined) {
            step = new PbcatStep('Question 4', 'legOfIntrsectCd');
        }
        else if (pedInfo.pedestrianMovementCd === undefined) {
            step = new PbcatStep('Question 5', 'pedestrianMovementCd');
        }
        else if (pedInfo.unusualCircumstancesCd === undefined) {
            step = new PbcatStep('Question 6', 'unusualCircumstancesCd');
        }
        else if (pedInfo.unusualVehicleTypeOrActionCd === undefined) {
            step = new PbcatStep('Question 7', 'unusualVehicleTypeOrActionCd');
        }
        else if (pedInfo.unusualPedActionCd === undefined) {
            step = new PbcatStep('Question 8', 'unusualPedActionCd');
        }
        else if (pedInfo.typicalPedActionCd === undefined) {
            step = new PbcatStep('Question 9', 'typicalPedActionCd');
        }
        else if (pedInfo.crossingRoadwayCd === undefined) {
            step = new PbcatStep('Question 10', 'crossingRoadwayCd');
        }
        else if (pedInfo.turnMergeCd === undefined) {
            step = new PbcatStep('Question 11', 'turnMergeCd');
        }

        step.items = [1, 2, 3, 4].map(i => new PbcatItem(i, i, `Option ${i}`));

        return step;
    }
}
