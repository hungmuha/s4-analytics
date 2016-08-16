import * as ng from '@angular/core';
import { PbcatItem, PbcatStep } from './shared';

@ng.Component({
    selector: 'pbcat-step',
    template: require('./pbcat-step.component.html')
})
export class PbcatStepComponent {
    @ng.Input() stepNumber: number;
}
