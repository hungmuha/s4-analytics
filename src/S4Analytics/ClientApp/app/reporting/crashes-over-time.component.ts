import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { CrashesOverTimeQuery } from './shared';

@Component({
    selector: 'crashes-over-time',
    templateUrl: './crashes-over-time.component.html'
})
export class CrashesOverTimeComponent implements OnInit {

    query = new CrashesOverTimeQuery();

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

    refresh() {
        let query: CrashesOverTimeQuery = {
            geographyId: undefined, // todo
            reportingAgencyId: undefined, // todo
            severity: this.selectedSeverities.length > 0
                ? {
                    fatality: _.includes(this.selectedSeverities, 'Fatal'),
                    injury: _.includes(this.selectedSeverities, 'Injury'),
                    propertyDamageOnly: _.includes(this.selectedSeverities, 'PDO')
                } : undefined,
            impairment: this.selectedImpairments.length > 0
                ? {
                    alcoholRelated: _.includes(this.selectedImpairments, 'Alcohol'),
                    drugRelated: _.includes(this.selectedImpairments, 'Drugs')
                } : undefined,
            bikePedRelated: this.selectedBikePedTypes.length > 0
                ? {
                    bikeRelated: _.includes(this.selectedBikePedTypes, 'Bike'),
                    pedRelated: _.includes(this.selectedBikePedTypes, 'Ped')
                } : undefined,
            cmvRelated: this.selectedCmvRelated !== undefined
                ? this.selectedCmvRelated === 'Yes'
                : undefined,
            codeable: this.selectedCodeable !== undefined
                ? this.selectedCodeable === 'Yes'
                : undefined,
            formType: this.selectedFormType !== undefined
                ? {
                    longForm: this.selectedFormType === 'Long',
                    shortForm: this.selectedFormType === 'Short'
                } : undefined
        }
        this.query = query;
    }
}
