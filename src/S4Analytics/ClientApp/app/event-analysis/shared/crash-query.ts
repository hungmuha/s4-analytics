export class CrashQuery {
    dateRange: {
        startDate: Date,
        endDate: Date
    };
    dayOfWeek?: number[];
    timeRange?: {
        startTime: Date,
        endTime: Date
    };
    dotDistrict?: number[];
    mpoTpo?: number[];
    county?: number[];
    city?: number[];
    customArea?: {
        x: number,
        y: number
    }[];
    customExtent?: {
        minX: number,
        minY: number,
        maxX: number,
        maxY: number
    };
    intersection?: {
        intersectionId: number,
        offsetInFeet: number,
        offsetDirection?: string[];
    };
    street?: {
        linkIds: number[],
        includeCrossStreets: boolean;
    };
    customNetwork?: number[];
    publicRoadOnly?: boolean;
    formType?: string[];
    codeableOnly?: boolean;
    reportingAgency?: number[];
    driverGender?: number[];
    driverAgeRange?: string[];
    pedestrianAgeRange?: string[];
    cyclistAgeRange?: string[];
    nonAutoModesOfTravel?: {
        pedestrian?: boolean,
        bicyclist?: boolean,
        moped?: boolean,
        motorcycle?: boolean;
    };
    sourcesOfTransport?: {
        ems?: boolean,
        lawEnforcement?: boolean,
        other?: boolean;
    };
    behavioralFactors?: {
        alcohol?: boolean,
        drugs?: boolean,
        distraction?: boolean,
        aggressiveDriving?: boolean;
    };
    commonViolations?: {
        speed?: boolean,
        redLight?: boolean,
        rightOfWay?: boolean,
        trafficControlDevice?: boolean,
        carelessDriving?: boolean,
        dui?: boolean;
    };
    vehicleType?: number[];
    crashTypeSimple?: string[];
    crashTypeDetailed?: number[];
    bikePedCrashType?: {
        bikePedCrashTypeIds: number[],
        includeUntyped?: boolean;
    };
    cmvConfiguration?: number[];
    environmentalCircumstance?: number[];
    roadCircumstance?: number[];
    firstHarmfulEvent?: number[];
    lightCondition?: number[];
    roadSystemIdentifier?: number[];
    weatherCondition?: number[];
    laneDepartures?: {
        offRoadAll?: boolean,
        offRoadRollover?: boolean,
        offRoadCollisionWithFixedObject?: boolean,
        crossedIntoOncomingTraffic?: boolean,
        sideswipe?: boolean;
    };
    otherCircumstances?: {
        hitAndRun?: boolean,
        schoolBusRelated?: boolean,
        withinCityLimits?: boolean,
        withinInterchange?: boolean,
        workZoneRelated?: boolean,
        workersInWorkZone?: boolean,
        lawEnforcementInWorkZone?: boolean;
    };
}
