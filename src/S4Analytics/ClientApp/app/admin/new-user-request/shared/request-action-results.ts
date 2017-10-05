export class RequestActionResults {
    public requestNumber: number;
    public approved: boolean;
    public rejectionReason: string;
    public accessBefore70Days: boolean;
    public contractEndDt: string;

    constructor(id: number) {
        this.requestNumber = id;
    }
}
