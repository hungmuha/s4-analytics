import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'crashes-over-time',
    templateUrl: './crashes-over-time.component.html'
})
export class CrashesOverTimeComponent implements OnInit {

    severities = ['Fatal', 'Injury', 'PDO'];
    impairments = ['Alcohol', 'Drugs'];
    bikePedTypes = ['Bike', 'Ped'];
    yesNo = ['Yes', 'No'];
    formTypes = ['Long', 'Short'];
    reportScopes = ['Year', 'Month', 'Day'];
    years: number[];

    selectedSeverities: string[] = [];
    selectedImpairments: string[] = [];
    selectedBikePedTypes: string[] = [];
    selectedCmvRelated?: string = undefined;
    selectedCodeable?: string = undefined;
    selectedFormType?: string = undefined;
    selectedReportScope: string;
    selectedYear: number;

    yearOnYear: boolean;
    alignByWeek: boolean;

    private currentYear = (new Date()).getFullYear();

    ngOnInit() {
        this.years = [this.currentYear, this.currentYear - 1, this.currentYear - 2, this.currentYear - 3];
        this.selectedReportScope = 'Year';
        this.selectedYear = this.currentYear;
        this.yearOnYear = true;
        this.alignByWeek = true;
    }
}
