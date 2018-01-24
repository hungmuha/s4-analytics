import { Component, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import * as _ from 'lodash';
import { DateTimeScope } from './shared';

@Component({
    selector: 'date-time-modal',
    templateUrl: './date-time-modal.component.html'
})
export class DateTimeModalComponent {

    @ViewChild('modal') modal: ElementRef;
    @Output() apply = new EventEmitter<DateTimeScope>();
    @Output() cancel = new EventEmitter<any>();
    @Input() scope: DateTimeScope;

    _scope: DateTimeScope;

    constructor(private modalService: NgbModal) { }

    open() {
        this._scope = _.cloneDeep(this.scope);
        this.modalService.open(this.modal).result.then((result) => {
            this.apply.emit(_.cloneDeep(this._scope));
        }, (reason) => {
            this.cancel.emit();
        });
    }
}
