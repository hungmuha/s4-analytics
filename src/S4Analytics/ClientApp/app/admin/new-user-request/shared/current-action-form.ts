import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

export class CurrentActionForm {
    modalRef: NgbModalRef;
    valid: boolean = true;

    public close() {
        this.modalRef.close();
    }
}
