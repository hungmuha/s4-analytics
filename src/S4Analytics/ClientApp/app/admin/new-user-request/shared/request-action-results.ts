export class RequestActionResults {
    public approved: boolean;
    public rejectionReason: string;
}

export class NewAgencyResults extends RequestActionResults {
    public lea: boolean;
    public accessBefore70Days: boolean;
}

export class NewNonFlAgencyResults extends RequestActionResults {
    public noReportAccess: boolean;
    public counties: string[];
    public expirationDt: Date;
}