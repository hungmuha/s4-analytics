import { Injectable } from '@angular/core';
import { PbcatPedestrianInfo } from './pbcat-ped-info.model';
import { PbcatCrashType } from './pbcat-crash-type.model';
import { PbcatStep } from './pbcat-step.model';
import { PbcatItem } from './pbcat-item.model';

const PED_INFO_ENDPOINT = '/api/pbcat/ped';
const PED_CRASH_TYPE_ENDPOINT = '/api/pbcat/ped/crashtype';
const PED_MAPPINGS_ENDPOINT = '/api/pbcat/ped/mappings';

@Injectable()
export class PbcatService {
    getPbcatPedestrianInfo(): PbcatPedestrianInfo {
        // GET /api/pbcat/ped/:hsmvRptNr
        return undefined;
    }

    savePbcatPedestrianInfo(getNextCrash: boolean = false): number {
        // POST /api/pbcat/ped
        //  PUT /api/pbcat/ped/:hsmvRptNr
        return undefined;
    }

    deletePbcatPedestrianInfo(): void {
        //  DELETE /api/pbcat/ped/:hsmvRptNr
    }

    calculatePedestrianCrashType(): PbcatCrashType {
        // GET /api/pbcat/ped/crashtype
        return undefined;
    }

    getPedestrianNextStep(): Promise<PbcatStep> {
        let step = new PbcatStep(1, 'Motorist Maneuver', 'Select the motorist\'s maneuver');
        step.items = [
            new PbcatItem(1, 'Left turn'),
            new PbcatItem(2, 'Right turn'),
            new PbcatItem(3, 'Straight through'),
            new PbcatItem(4, 'Unknown')
        ];
        return Promise.resolve<PbcatStep>(step);
    }
}
