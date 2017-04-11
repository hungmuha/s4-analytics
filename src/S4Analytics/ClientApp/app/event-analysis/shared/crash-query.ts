export class DateRange {
    startDate: Date;
    endDate: Date;
}

export class TimeRange {
    startTime: Date;
    endTime: Date;
}

export class Coordinates {
    x: number;
    y: number;
}

export class Extent {
    minX: number;
    minY: number;
    maxX: number;
    maxY: number;
}

export class IntersectionParameters {
    intersectionId: number;
    offsetInFeet: number;
    offsetDirection?: string[];
}

export class StreetParameters {
    linkIds: number[];
    includeCrossStreets: boolean;
}

export class NonAutoModesOfTravel {
    pedestrian?: boolean;
    bicyclist?: boolean;
    moped?: boolean;
    motorcycle?: boolean;
}

export class SourcesOfTransport {
    ems?: boolean;
    lawEnforcement?: boolean;
    other?: boolean;
}

export class BehavioralFactors {
    alcohol?: boolean;
    drugs?: boolean;
    distraction?: boolean;
    aggressiveDriving?: boolean;
}

export class CommonViolations {
    speed?: boolean;
    redLight?: boolean;
    rightOfWay?: boolean;
    trafficControlDevice?: boolean;
    carelessDriving?: boolean;
    dui?: boolean;
}

export class BikePedCrashType {
    bikePedCrashTypeIds: number[];
    includeUntyped?: boolean;
}

export class LaneDepartures {
    offRoadAll?: boolean;
    offRoadRollover?: boolean;
    offRoadCollisionWithFixedObject?: boolean;
    crossedIntoOncomingTraffic?: boolean;
    sideswipe?: boolean;
}

export class OtherCircumstances {
    hitAndRun?: boolean;
    schoolBusRelated?: boolean;
    withinCityLimits?: boolean;
    withinInterchange?: boolean;
    workZoneRelated?: boolean;
    workersInWorkZone?: boolean;
    lawEnforcementInWorkZone?: boolean;
}

export class CrashQuery {
    dateRange: DateRange;
    dayOfWeek?: number[];
    timeRange?: TimeRange;
    dotDistrict?: number[];
    mpoTpo?: number[];
    county?: number[];
    city?: number[];
    customArea?: Coordinates[];
    customExtent?: Extent;
    intersection?: IntersectionParameters;
    street?: StreetParameters;
    customNetwork?: number[];
    publicRoadOnly?: boolean;
    formType?: string[];
    codeableOnly?: boolean;
    reportingAgency?: number[];
    driverGender?: number[];
    driverAgeRange?: string[];
    pedestrianAgeRange?: string[];
    cyclistAgeRange?: string[];
    nonAutoModesOfTravel?: NonAutoModesOfTravel;
    sourcesOfTransport?: SourcesOfTransport;
    behavioralFactors?: BehavioralFactors;
    commonViolations?: CommonViolations;
    vehicleType?: number[];
    crashTypeSimple?: string[];
    crashTypeDetailed?: number[];
    bikePedCrashType?: BikePedCrashType;
    cmvConfiguration?: number[];
    environmentalCircumstance?: number[];
    roadCircumstance?: number[];
    firstHarmfulEvent?: number[];
    lightCondition?: number[];
    roadSystemIdentifier?: number[];
    weatherCondition?: number[];
    laneDepartures?: LaneDepartures;
    otherCircumstances?: OtherCircumstances;
}
