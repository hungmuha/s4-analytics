import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

export class CurrentActionForm {
    form: NgbModalRef;
    valid: boolean = true;

    public close() {
        this.form.close();
    }
}
