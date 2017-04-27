export class RequestActionResults {
    public requestNumber: number;
    public approved: boolean;
    public rejectionReason: string;
    public lea: boolean;
    public accessBefore70Days: boolean;

    constructor(id: number) {
        this.requestNumber = id;
    }
}
