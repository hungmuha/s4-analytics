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
        this.categories = report.categories;
        this.series = report.series;
        this.maxDate = new Date(report.maxDate);
    }
}
