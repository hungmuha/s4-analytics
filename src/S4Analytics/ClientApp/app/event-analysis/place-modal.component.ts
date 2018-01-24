import { Component, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import { LookupKeyAndName } from '../shared';
import { PlaceScope, EventAnalysisStateService } from './shared';

@Component({
    selector: 'place-modal',
    templateUrl: './place-modal.component.html'
})
export class PlaceModalComponent {

    @ViewChild('modal') modal: ElementRef;
    @Output() apply = new EventEmitter<PlaceScope>();
    @Output() cancel = new EventEmitter<any>();
    @Input() scope: PlaceScope;

    geoExtent: 'Statewide' | 'County' | 'City';
    filteredCounties: LookupKeyAndName[];
    filteredCities: LookupKeyAndName[];
    selectedCounties: LookupKeyAndName[];
    selectedCities: LookupKeyAndName[];

    constructor(
        private modalService: NgbModal,
        private state: EventAnalysisStateService) { }

    open() {
        this.parseScope();
        this.modalService.open(this.modal).result.then((result) => {
            let scope = this.generateScope();
            this.apply.emit(scope);
        }, (reason) => {
            this.cancel.emit();
        });
    }

    filterCounties(filter: string): void {
        this.filteredCounties = filter.length > 0
            ? this.state.allCounties.filter(s => s.name.toLowerCase().indexOf(filter.toLowerCase()) !== -1).slice(0, 10)
            : [];
    }

    filterCities(filter: string): void {
        this.filteredCities = filter.length > 0
            ? this.state.allCities.filter(s => s.name.toLowerCase().indexOf(filter.toLowerCase()) !== -1).slice(0, 10)
            : [];
    }

    private parseScope() {
        this.selectedCounties = [];
        this.selectedCities = [];

        if (this.scope.county !== undefined && this.scope.county.length > 0) {
            this.geoExtent = 'County';
            this.selectedCounties = _.filter(this.state.allCounties, kn => _.includes(this.scope.county, kn.key));
        }
        else if (this.scope.city !== undefined && this.scope.city.length > 0) {
            this.geoExtent = 'City';
            this.selectedCities = _.filter(this.state.allCities, kn => _.includes(this.scope.city, kn.key));
        }
        else {
            this.geoExtent = 'Statewide';
        }
    }

    private generateScope(): PlaceScope {
        let scope = new PlaceScope();
        switch (this.geoExtent) {
            case 'County':
                scope.county = this.selectedCounties.map(kn => kn.key);
                break;
            case 'City':
                scope.city = this.selectedCities.map(kn => kn.key);
                break;
            case 'Statewide':
            default:
                break;
        }
        return scope;
    }
}
