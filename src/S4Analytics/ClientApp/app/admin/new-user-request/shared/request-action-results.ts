export class RequestActionResults {
    public requestNumber: number;
    public approved: boolean;
    public rejectionReason: string;

    constructor(id: number) {
        this.requestNumber = id;
    }
}

export class NewAgencyActionResults extends RequestActionResults {
    public lea: boolean;
    public accessBefore70Days: boolean;
}

export class NewConsultantActionResults extends RequestActionResults {
    public accessBefore70Days: boolean;
}
