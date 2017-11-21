import { Component, Input, Output, EventEmitter } from '@angular/core';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';

@Component({
    selector: 'checkbox-group',
    providers: [makeProvider(CheckboxGroupComponent)],
    template: `<fieldset class="form-group">
                    <h6><b>{{groupLabel}}</b> <a *ngIf="hasHelp" href="javascript:void(0)" (click)="showHelp.emit()"><span class="fa fa-question-circle"></span></a></h6>
                    <div class="form-check" [class.form-check-inline]="inline" *ngFor="let item of items">
                        <label class="form-check-label">
                            <input type="checkbox"
                                    class="form-check-input"
                                    [name]="name"
                                    [checked]="isChecked(item)"
                                    (click)="toggle(item)" />
                            <span ngbTooltip="{{getItemTooltip(item)}}" container="body" placement="right" (click)="toggle(item)">
                                {{getItemLabel(item)}}
                            </span>
                        </label>
                    </div>
                </fieldset>`
})
export class CheckboxGroupComponent extends AbstractValueAccessor {
    @Input() name: string;
    @Input() groupLabel: string;
    @Input() items: any[];
    @Input() labelMember?: string;
    @Input() tooltipMember?: string;
    @Input() valueMember?: string;
    @Input() hasHelp?: boolean = false;
    @Input() inline?: boolean = true;
    @Output() showHelp = new EventEmitter();

    get isAllChecked(): boolean {
        return this.selectedItemValues !== undefined && this.selectedItemValues.length === 0;
    }

    private get selectedItemValues(): any[] {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        return this.value;
    }

    private set selectedItemValues(value: any[]) {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        this.value = value;
    }

    onValueChange(): void {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        if (this.value === undefined) {
            this.value = [];
        }
    }

    toggleAll() {
        if (!this.isAllChecked) {
            this.selectedItemValues = [];
        }
    }

    isChecked(item: any): boolean {
        let itemValue = this.getItemValue(item);
        return _.includes(this.selectedItemValues, itemValue);
    }

    toggle(item: any) {
        let itemValue = this.getItemValue(item);
        let newItemValues = [...this.selectedItemValues]; // create a copy
        let isChecked = _.includes(newItemValues, itemValue);
        if (isChecked) {
            _.remove(newItemValues, v => v === itemValue);
        }
        else {
            newItemValues.push(itemValue);
        }
        // to allow change detection to work as expected, we must overwrite
        // selectedItemValues rather than remove or insert an item directly
        this.selectedItemValues = newItemValues;
    }

    getItemLabel(item: any): any {
        let label = this.labelMember !== undefined
            ? item[this.labelMember]
            : item;
        return label;
    }

    getItemTooltip(item: any): any {
        let label = this.tooltipMember !== undefined
            ? item[this.tooltipMember]
            : undefined;
        return label;
    }

    private getItemValue(item: any): any {
        let value = this.valueMember !== undefined
            ? item[this.valueMember]
            : item;
        return value;
    }
}
