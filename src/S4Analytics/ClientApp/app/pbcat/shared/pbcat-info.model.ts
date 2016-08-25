export abstract class PbcatInfo {
    rightTurnOnRedCd: number;
}

export class PbcatPedestrianInfo extends PbcatInfo {
    backingVehicleCd: number;
    crashLocationCd: number;
    crossingDrivewayCd: number;
    crossingRoadwayCd: number;
    failureToYieldCd: number;
    legOfIntrsectCd: number;
    motoristDirTravelCd: number;
    motoristManeuverCd: number;
    nonRoadwayLocationCd: number;
    otherPedActionCd: number;
    pedestrianDirTravelCd: number;
    pedestrianFailedToYieldCd: number;
    pedestrianInRoadwayCd: number;
    pedestrianMovementCd: number;
    pedestrianPositionCd: number;
    turnMergeCd: number;
    typicalPedActionCd: number;
    unusualCircumstancesCd: number;
    unusualPedActionCd: number;
    unusualVehicleTypeOrActionCd: number;
    waitingToCrossCd: number;
    walkingAlongRoadwayCd: number;
}

export class PbcatBicyclistInfo extends PbcatInfo {
    bicyclistDirCd: any;
    bicyclistFailedToClearCd: any;
    bicyclistOvertakingCd: any;
    bicyclistPositionCd: any;
    bicyclistRideOutCd: any;
    bicyclistTurnedMergedCd: any;
    crashLocationCd: any;
    crossingPathsIntrsectCd: any;
    crossingPathsNonIntrsectCd: any;
    headOnCrashCd: any;
    initialApproachPathsCd: any;
    intentionalCrashCd: any;
    intrsectCircumstancesCd: any;
    lossOfControlCd: any;
    motoristDriveOutCd: any;
    motoristOvertakingCd: any;
    motoristTurnedMergedCd: any;
    parallelPathsCd: any;
    turningErrorCd: any;
    typeTrafficControlCd: any;
    unusualCircumstancesCd: any;
}
