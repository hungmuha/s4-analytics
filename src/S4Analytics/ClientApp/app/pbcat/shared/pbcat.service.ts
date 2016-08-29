import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { ParticipantType } from './pbcat.enums';
import { PbcatConfig } from './pbcat-config.d.ts';
import { PbcatCrashType } from './pbcat-crash-type';
import { PbcatInfo, PbcatBicyclistInfo, PbcatPedestrianInfo } from './pbcat-info';

@Injectable()
export class PbcatService {
    private cachedPedConfig: PbcatConfig;
    private cachedBikeConfig: PbcatConfig;

    constructor(private http: Http) { }

    handleError() {
        // todo: implement error handling
    }

    getConfiguration(participantType: ParticipantType): Promise<PbcatConfig> {
        // return ped config if it was previously cached
        if (participantType === ParticipantType.Pedestrian && this.cachedPedConfig !== undefined) {
            return Promise.resolve(this.cachedPedConfig);
        }
        // return bike config if it was previously cached
        else if (participantType === ParticipantType.Bicyclist && this.cachedBikeConfig !== undefined) {
            return Promise.resolve(this.cachedBikeConfig);
        }
        // otherwise retrieve the config from the server and cache it
        else {
            let jsonUrl = participantType === ParticipantType.Pedestrian
                ? 'json/pbcat-ped.json'
                : 'json/pbcat-bike.json';
            return this.http
                .get(jsonUrl)
                .toPromise()
                .then(response => response.json() as PbcatConfig)
                .then(config => this.cacheConfig(participantType, config))
                .catch(this.handleError);
        }
    }

    getPbcatInfo(participantType: ParticipantType, hsmvReportNumber: number): Promise<PbcatInfo> {
        // GET /api/pbcat/:bikeOrPed/:hsmvRptNr
        // todo: reconstruct stepHistory for previously typed crashes
        let info = participantType === ParticipantType.Pedestrian
            ? new PbcatPedestrianInfo()
            : new PbcatBicyclistInfo();
        return Promise.resolve(info);
    }

    savePbcatInfo(
        participantType: ParticipantType,
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        getNextCrash: boolean = false): Promise<[ParticipantType, number]> {
        // POST /api/pbcat/:bikeOrPed
        //  PUT /api/pbcat/:bikeOrPed/:hsmvRptNr
        // mock get the actual next hsmv report number
        if (getNextCrash) {
            let retVal = [participantType, getNextCrash ? hsmvReportNumber + 1 : undefined];
            return Promise.resolve(retVal);
        }
    }

    deletePbcatInfo(hsmvReportNumber: number): Promise<void> {
        //  DELETE /api/pbcat/:bikeOrPed/:hsmvRptNr
        return Promise.resolve();
    }

    calculateCrashType(pbcatInfo: PbcatInfo): Promise<PbcatCrashType> {
        // GET /api/pbcat/:bikeOrPed/crashtype
        // mock crash type
        let crashType = new PbcatCrashType();
        crashType.crashTypeNbr = 781;
        crashType.crashTypeDesc = 'Motorist Left Turn - Parallel Paths';
        crashType.crashGroupNbr = 790;
        crashType.crashGroupDesc = 'Crossing Roadway - Vehicle Turning';
        crashType.crashTypeExpanded = 12781;
        crashType.crashGroupExpanded = 12790;
        return Promise.resolve(crashType);
    }

    private cacheConfig(participantType: ParticipantType, config: PbcatConfig): PbcatConfig {
        if (participantType === ParticipantType.Pedestrian) {
            this.cachedPedConfig = config;
        }
        else if (participantType === ParticipantType.Bicyclist) {
            this.cachedBikeConfig = config;
        }
        // this method is used in a promise chain so it must return a PbcatConfig
        return config;
    }
}
