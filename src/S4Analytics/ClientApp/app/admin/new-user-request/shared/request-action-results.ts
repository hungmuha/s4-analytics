export class RequestActionResults {
    public requestNumber: number;
    public approved: boolean;
    public rejectionReason: string;
    public accessBefore70Days: boolean;
    public contractEndDt: Date;

    constructor(id: number) {
        this.requestNumber = id;
    }
}
