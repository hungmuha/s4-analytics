import { Component } from '@angular/core';
import { NewUserRequestStateService } from './shared';
import { RequestActionComponent } from './request-action.component';

@Component({
    selector: 'new-consultant-component',
    templateUrl: './new-consultant.component.html'
})

export class NewConsultantComponent extends RequestActionComponent {

    constructor(public state: NewUserRequestStateService) {
        super(state);
    }

    openContractViewer() {
        this.state.contractViewerWindow = window.open('', '', 'width=200,height=100');
    }
}

