export class RequestActionResults {
    public requestNumber: number;
    public approved: boolean;
    public rejectionReason: string;
    public lea: boolean;
    public accessBefore70Days: boolean;
    public agencyCreated: boolean = true;

    constructor(id: number) {
        this.requestNumber = id;
    }
}
