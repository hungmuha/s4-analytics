import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import '../../rxjs-operators';
import { PbcatFlow, FlowType } from './pbcat-flow';
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

export class NextCrashInfo {
    constructor(
        public hsmvReportNumber: number,
        public flowType: FlowType) { }
}

export class PbcatInfoWithExists {
    constructor(
        public pbcatInfo: PbcatInfo,
        public exists: boolean) { }
}

@Injectable()
export class PbcatService {
    private cachedPedConfig: PbcatConfig;
    private cachedBikeConfig: PbcatConfig;

    constructor(private http: Http) { }

    getConfiguration(flowType: FlowType): Observable<PbcatConfig> {
        // return ped config if it was previously cached
        if (flowType === FlowType.Pedestrian && this.cachedPedConfig !== undefined) {
            return Observable.of(this.cachedPedConfig);
        }
        // return bike config if it was previously cached
        else if (flowType === FlowType.Bicyclist && this.cachedBikeConfig !== undefined) {
            return Observable.of(this.cachedBikeConfig);
        }
        // otherwise retrieve the config from the server and cache it
        else {
            let jsonUrl = flowType === FlowType.Pedestrian
                ? 'json/pbcat-ped.json'
                : 'json/pbcat-bike.json';
            return this.http
                .get(jsonUrl)
                .map(response => this.extractData<PbcatConfig>(response))
                .do(config => this.cacheConfig(flowType, config))
                .catch(this.handleError);
        }
    }

    pbcatInfoError(flowType: FlowType, error: any): Observable<any> {
        // if no record was found, create an empty one
        if (error.status === 404) {
            return Observable.of(
                flowType === FlowType.Pedestrian
                    ? new PbcatInfoWithExists(new PbcatPedestrianInfo(), false)
                    : new PbcatInfoWithExists(new PbcatBicyclistInfo(), false)
            );
        }
        else {
            return this.handleError(error);
        }
    }

    getPbcatInfo(flowType: FlowType, hsmvReportNumber: number): Observable<PbcatInfoWithExists> {
        // GET api/pbcat/:bikeOrPed/:hsmvRptNr
        let bikeOrPed = this.getBikeOrPed(flowType);
        let url = `api/pbcat/${bikeOrPed}/${hsmvReportNumber}`;
        return this.http
            .get(url)
            .map(response => this.extractData<PbcatInfo>(response))
            .map(data => new PbcatInfoWithExists(data, true))
            .catch(error => this.pbcatInfoError(flowType, error));
        // todo: reconstruct stepHistory for previously typed crashes
    }

    createPbcatInfo(flow: PbcatFlow): Observable<NextCrashInfo> {
        // POST api/pbcat/:bikeOrPed
        let bikeOrPed = this.getBikeOrPed(flow.flowType);
        let url = `api/pbcat/${bikeOrPed}`;
        let wrapper = flow.flowType === FlowType.Pedestrian
            ? new PedestrianInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatPedestrianInfo, flow.crashType)
            : new BicyclistInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatBicyclistInfo, flow.crashType);
        // todo: get the actual next report info
        let nextFlowType: FlowType = undefined;
        let nextHsmvNumber: number = undefined;
        return this.http
            .post(url, wrapper)
            .map(response => new NextCrashInfo(nextHsmvNumber, nextFlowType))
            .catch(this.handleError);
    }

    updatePbcatInfo(flow: PbcatFlow): Observable<NextCrashInfo> {
        // PUT api/pbcat/:bikeOrPed/:hsmvRptNr
        let bikeOrPed = this.getBikeOrPed(flow.flowType);
        let url = `api/pbcat/${bikeOrPed}/${flow.hsmvReportNumber}`;
        let wrapper = flow.flowType === FlowType.Pedestrian
            ? new PedestrianInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatPedestrianInfo, flow.crashType)
            : new BicyclistInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatBicyclistInfo, flow.crashType);
        // todo: get the actual next report info
        let nextFlowType: FlowType = undefined;
        let nextHsmvNumber: number = undefined;
        return this.http
            .put(url, wrapper)
            .map(response => new NextCrashInfo(nextHsmvNumber, nextFlowType))
            .catch(this.handleError);
    }

    calculateCrashType(flowType: FlowType, pbcatInfo: PbcatInfo): Observable<PbcatCrashType> {
        // POST api/pbcat/:bikeOrPed/crashtype
        let bikeOrPed = this.getBikeOrPed(flowType);
        let url = `api/pbcat/${bikeOrPed}/crashtype`;
        return this.http
            .post(url, pbcatInfo)
            .map(response => this.extractData<PbcatCrashType>(response))
            .catch(this.handleError);
    }

    private getBikeOrPed(flowType: FlowType): string {
        if (flowType === FlowType.Pedestrian) {
            return 'ped';
        }
        else if (flowType === FlowType.Bicyclist) {
            return 'bike';
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

    private extractData<T>(response: Response): T {
        let body = response.json();
        let data = body.data || body || {};
        return data as T;
    }

    private cacheConfig(flowType: FlowType, config: PbcatConfig) {
        if (flowType === FlowType.Pedestrian) {
            this.cachedPedConfig = config;
        }
        else if (flowType === FlowType.Bicyclist) {
            this.cachedBikeConfig = config;
        }
    }
}
