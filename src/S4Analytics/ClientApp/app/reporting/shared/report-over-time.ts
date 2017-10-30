export class ReportSeries
{
    name: string;
    data: number[];
}

export class ReportOverTimeByYear
{
    categories: string[];
    series: ReportSeries[];
}
