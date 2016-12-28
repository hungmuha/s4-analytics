import { Component } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';
import { KeepSilverlightAliveService } from '../../keep-silverlight-alive.service';
import { OptionsService, Options } from '../../options.service';
import { ModalDismissReasons, NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'request-action-modal',
    template: require('./request-action.component.html')
})
export class RequestActionComponent {
    closeResult: string;
    constructor(private modalService: NgbModal) { }

    openActionModal(content: any) {
        this.modalService.open(content).result.then((result) => {
            this.closeResult = `${result}`;
        }, (reason) => {
            this.closeResult = `${this.getDismissReason(reason)}`;
            });
    }

    private getDismissReason(reason: any): string {
        if (reason === ModalDismissReasons.ESC) {
            return 'pressed ESC';
        } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
            return 'clicked backdrop';
        } else {
            return `${reason}`;
        }
    }

}

