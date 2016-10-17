export abstract class PbcatInfo {
    rightTurnOnRedCd: string;
    notes: string;
}

export class PbcatPedestrianInfo extends PbcatInfo {
    backingVehicleCd: string;
    crashLocationCd: string;
    crossingDrivewayCd: string;
    crossingRoadwayCd: string;
    failureToYieldCd: string;
    legOfIntrsectCd: string;
    motoristDirTravelCd: string;
    motoristManeuverCd: string;
    nonRoadwayLocationCd: string;
    otherPedActionCd: string;
    pedestrianDirTravelCd: string;
    pedestrianFailedToYieldCd: string;
    pedestrianInRoadwayCd: string;
    pedestrianMovementCd: string;
    pedestrianPositionCd: string;
    turnMergeCd: string;
    typicalPedActionCd: string;
    unusualCircumstancesCd: string;
    unusualPedActionCd: string;
    unusualVehicleTypeOrActionCd: string;
    waitingToCrossCd: string;
    walkingAlongRoadwayCd: string;
}

export class PbcatBicyclistInfo extends PbcatInfo {
    bicyclistDirCd: string;
    bicyclistFailedToClearCd: string;
    bicyclistOvertakingCd: string;
    bicyclistPositionCd: string;
    bicyclistRideOutCd: string;
    bicyclistTurnedMergedCd: string;
    crashLocationCd: string;
    crossingPathsIntrsectCd: string;
    crossingPathsNonIntrsectCd: string;
    headOnCrashCd: string;
    initialApproachPathsCd: string;
    intentionalCrashCd: string;
    intrsectCircumstancesCd: string;
    lossOfControlCd: string;
    motoristDriveOutCd: string;
    motoristOvertakingCd: string;
    motoristTurnedMergedCd: string;
    parallelPathsCd: string;
    turningErrorCd: string;
    typeTrafficControlCd: string;
    unusualCircumstancesCd: string;
}
