import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { FlowType } from './pbcat.state';
import { PbcatConfig } from './pbcat-config.d.ts';
import { PbcatCrashType } from './pbcat-crash-type';
import { PbcatInfo, PbcatBicyclistInfo, PbcatPedestrianInfo } from './pbcat-info';

class PedestrianInfoWrapper {
    constructor(
        public hsmvReportNumber: number,
        public pedestrianInfo: PbcatPedestrianInfo,
        public pedestrianCrashType: PbcatCrashType
    ) { }
}

class BicyclistInfoWrapper {
    constructor(
        public hsmvReportNumber: number,
        public bicyclistInfo: PbcatBicyclistInfo,
        public bicyclistCrashType: PbcatCrashType
    ) { }
}

@Injectable()
export class PbcatService {
    private cachedPedConfig: PbcatConfig;
    private cachedBikeConfig: PbcatConfig;

    constructor(private http: Http) { }

    handleError(error: any): Promise<void> {
        // super generic error handling
        let errMsg = (error.message) ? error.message :
            error.status ? `${error.status} - ${error.statusText}` : 'Server error';
        console.error(errMsg); // log to console
        return Promise.reject(errMsg);
    }

    getConfiguration(flowType: FlowType): Promise<PbcatConfig> {
        // return ped config if it was previously cached
        if (flowType === FlowType.Pedestrian && this.cachedPedConfig !== undefined) {
            return Promise.resolve(this.cachedPedConfig);
        }
        // return bike config if it was previously cached
        else if (flowType === FlowType.Bicyclist && this.cachedBikeConfig !== undefined) {
            return Promise.resolve(this.cachedBikeConfig);
        }
        // otherwise retrieve the config from the server and cache it
        else {
            let jsonUrl = flowType === FlowType.Pedestrian
                ? 'json/pbcat-ped.json'
                : 'json/pbcat-bike.json';
            return this.http
                .get(jsonUrl)
                .toPromise()
                .then(response => response.json() as PbcatConfig)
                .then(config => this.cacheConfig(flowType, config))
                .catch(this.handleError);
        }
    }

    pbcatInfoError(flowType: FlowType, error: any): Promise<any> {
        // if no record was found, create an empty one
        if (error.status === 404) {
            let exists = false;
            return Promise.resolve(
                flowType === FlowType.Pedestrian
                    ? [new PbcatPedestrianInfo(), exists]
                    : [new PbcatBicyclistInfo(), exists]
            );
        }
        else {
            return this.handleError(error);
        }
    }

    getPbcatInfo(flowType: FlowType, hsmvReportNumber: number): Promise<[PbcatInfo, boolean]> {
        // GET /api/pbcat/:bikeOrPed/:hsmvRptNr
        let bikeOrPed = flowType === FlowType.Pedestrian ? 'ped' : 'bike';
        let url = `/api/pbcat/${bikeOrPed}/${hsmvReportNumber}`;
        let exists = true;
        return this.http
            .get(url)
            .toPromise()
            .then(response => [response.json() as PbcatInfo, exists])
            .catch(error => this.pbcatInfoError(flowType, error));
        // todo: reconstruct stepHistory for previously typed crashes
    }

    createPbcatInfo(
        flowType: FlowType,
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        crashType: PbcatCrashType,
        getNextCrash: boolean = false): Promise<[FlowType, number]> {
        // POST /api/pbcat/:bikeOrPed
        let bikeOrPed = flowType === FlowType.Pedestrian ? 'ped' : 'bike';
        let url = `/api/pbcat/${bikeOrPed}`;
        let wrapper = flowType === FlowType.Pedestrian
            ? new PedestrianInfoWrapper(hsmvReportNumber, pbcatInfo as PbcatPedestrianInfo, crashType)
            : new BicyclistInfoWrapper(hsmvReportNumber, pbcatInfo as PbcatBicyclistInfo, crashType);
        // todo: get the actual next report info
        let nextFlowType = getNextCrash ? FlowType.Pedestrian : undefined;
        let nextHsmvNumber = getNextCrash ? hsmvReportNumber + 1 : undefined;
        return this.http
            .post(url, wrapper)
            .toPromise()
            .then(response => [nextFlowType, nextHsmvNumber])
            .catch(this.handleError);
    }

    updatePbcatInfo(
        flowType: FlowType,
        hsmvReportNumber: number,
        pbcatInfo: PbcatInfo,
        crashType: PbcatCrashType,
        getNextCrash: boolean = false): Promise<[FlowType, number]> {
        // PUT /api/pbcat/:bikeOrPed/:hsmvRptNr
        let bikeOrPed = flowType === FlowType.Pedestrian ? 'ped' : 'bike';
        let url = `/api/pbcat/${bikeOrPed}/${hsmvReportNumber}`;
        let wrapper = flowType === FlowType.Pedestrian
            ? new PedestrianInfoWrapper(hsmvReportNumber, pbcatInfo as PbcatPedestrianInfo, crashType)
            : new BicyclistInfoWrapper(hsmvReportNumber, pbcatInfo as PbcatBicyclistInfo, crashType);
        // todo: get the actual next report info
        let nextFlowType = getNextCrash ? FlowType.Pedestrian : undefined;
        let nextHsmvNumber = getNextCrash ? hsmvReportNumber + 1 : undefined;
        return this.http
            .put(url, wrapper)
            .toPromise()
            .then(response => [nextFlowType, nextHsmvNumber])
            .catch(this.handleError);
    }

    deletePbcatInfo(flowType: FlowType, hsmvReportNumber: number): Promise<void> {
        //  DELETE /api/pbcat/:bikeOrPed/:hsmvRptNr
        let bikeOrPed = flowType === FlowType.Pedestrian ? 'ped' : 'bike';
        let url = `/api/pbcat/${bikeOrPed}/${hsmvReportNumber}`;
        return this.http
            .delete(url)
            .toPromise()
            .then(response => undefined)
            .catch(this.handleError);
    }

    calculateCrashType(flowType: FlowType, pbcatInfo: PbcatInfo): Promise<PbcatCrashType> {
        // POST /api/pbcat/:bikeOrPed/crashtype
        let bikeOrPed = flowType === FlowType.Pedestrian ? 'ped' : 'bike';
        let url = `/api/pbcat/${bikeOrPed}/crashtype`;
        return this.http
            .post(url, pbcatInfo)
            .toPromise()
            .then(response => response.json() as PbcatCrashType)
            .catch(this.handleError);
    }

    private cacheConfig(flowType: FlowType, config: PbcatConfig): PbcatConfig {
        if (flowType === FlowType.Pedestrian) {
            this.cachedPedConfig = config;
        }
        else if (flowType === FlowType.Bicyclist) {
            this.cachedBikeConfig = config;
        }
        // this method is used in a promise chain so it must return a PbcatConfig
        return config;
    }
}
