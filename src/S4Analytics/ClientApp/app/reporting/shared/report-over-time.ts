export class ReportSeries {
    id: string;
    name: string;
    data: number[];
}

export class ReportOverTime {
    categories: string[];
    series: ReportSeries[];
    maxDate: Date;

    constructor(report: ReportOverTime) {
        // merge data from the api
        Object.assign(this, report);
        // rest api represents dates as strings at runtime; convert
        this.maxDate = new Date(this.maxDate);
    }
}
