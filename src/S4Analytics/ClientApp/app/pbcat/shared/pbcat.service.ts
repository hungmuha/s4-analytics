import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import '../../rxjs-operators';
import { PbcatFlow, FlowType } from './pbcat-flow';
import { PbcatConfig } from './pbcat-config.d';
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

export class PbcatParticipantInfo {
    hasPedestrianParticipant: boolean;
    hasBicyclistParticipant: boolean;
    hasPedestrianTyping: boolean;
    hasBicyclistTyping: boolean;
}

export class NextCrashInfo {
    constructor(
        public hsmvReportNumber: number,
        public flowType: FlowType) { }
}

@Injectable()
export class PbcatService {
    private cachedConfig: PbcatConfig;

    constructor(private http: Http) { }

    getConfiguration(): Observable<PbcatConfig> {
        // return cached config if available
        if (this.cachedConfig !== undefined) {
            return Observable.of(this.cachedConfig);
        }
        // otherwise retrieve the config from the server and cache it
        else {
            let url = 'json/pbcat-config.json';
            return this.http
                .get(url)
                .map(response => this.extractData<PbcatConfig>(response))
                .do(config => this.cachedConfig = config)
                .catch(this.handleError);
        }
    }

    getSession(token: string): Observable<number[]> {
        let url = `api/pbcat/session/${token}`;
        return this.http.get(url)
            .map(response => this.extractData<number[]>(response))
            .catch(this.handleError);
    }

    getParticipantInfo(hsmvReportNumber: number): Observable<PbcatParticipantInfo> {
        let url = `api/pbcat/${hsmvReportNumber}`;
        return this.http
            .get(url)
            .map(response => this.extractData<PbcatParticipantInfo>(response))
            .catch(this.handleError);
    }

    getPbcatInfo(participantInfo: PbcatParticipantInfo, hsmvReportNumber: number): Observable<PbcatInfo> {
        // GET api/pbcat/:bikeOrPed/:hsmvRptNr
        if (participantInfo.hasPedestrianTyping || participantInfo.hasBicyclistTyping) {
            let bikeOrPed = participantInfo.hasPedestrianTyping ? 'ped' : 'bike';
            let url = `api/pbcat/${bikeOrPed}/${hsmvReportNumber}`;
            return this.http
                .get(url)
                .map(response => this.extractData<PbcatInfo>(response))
                .catch(this.handleError);
        }
        else {
            return Observable.of(undefined);
        }
    }

    createPbcatInfo(flow: PbcatFlow): Observable<void> {
        // POST api/pbcat/:bikeOrPed
        let bikeOrPed = this.getBikeOrPed(flow.flowType);
        let url = `api/pbcat/${bikeOrPed}`;
        let wrapper = flow.flowType === FlowType.Pedestrian
            ? new PedestrianInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatPedestrianInfo, flow.crashType)
            : new BicyclistInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatBicyclistInfo, flow.crashType);
        return this.http
            .post(url, wrapper)
            .map(response => undefined)
            .catch(this.handleError);
    }

    updatePbcatInfo(flow: PbcatFlow): Observable<void> {
        // PUT api/pbcat/:bikeOrPed/:hsmvRptNr
        let bikeOrPed = this.getBikeOrPed(flow.flowType);
        let url = `api/pbcat/${bikeOrPed}/${flow.hsmvReportNumber}`;
        let wrapper = flow.flowType === FlowType.Pedestrian
            ? new PedestrianInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatPedestrianInfo, flow.crashType)
            : new BicyclistInfoWrapper(flow.hsmvReportNumber, flow.pbcatInfo as PbcatBicyclistInfo, flow.crashType);
        return this.http
            .put(url, wrapper)
            .map(response => undefined)
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
}
