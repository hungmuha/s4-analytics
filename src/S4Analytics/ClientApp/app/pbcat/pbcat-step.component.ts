import * as ng from '@angular/core';
import { PbcatItem, PbcatStep } from './shared';

@ng.Component({
    selector: 'pbcat-step',
    template: require('./pbcat-step.component.html')
})
export class PbcatStepComponent {
    @ng.Input() stepNumber: number;
    private step: PbcatStep;

    constructor() {
        this.step = new PbcatStep(1, 'Motorist Maneuver', 'Select the motorist\'s maneuver');
        this.step.items = [
            new PbcatItem(1, 'Left turn'),
            new PbcatItem(2, 'Right turn'),
            new PbcatItem(3, 'Straight through'),
            new PbcatItem(4, 'Unknown')
        ];
    }
}
