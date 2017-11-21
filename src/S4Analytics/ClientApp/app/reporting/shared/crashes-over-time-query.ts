export class CrashesOverTimeQuery {
    geographyId?: number;
    reportingAgencyId?: number;
    severity?: {
        propertyDamageOnly: boolean,
        injury: boolean,
        fatality: boolean
    };
    impairment?: {
        alcoholRelated: boolean,
        drugRelated: boolean
    };
    bikePedRelated?: {
        bikeRelated: boolean,
        pedRelated: boolean
    };
    cmvRelated?: boolean;
    codeable?: boolean;
    formType?: {
        longForm: boolean,
        shortForm: boolean
    };
}
