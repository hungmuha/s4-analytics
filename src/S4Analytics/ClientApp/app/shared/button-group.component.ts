import { Component, Input } from '@angular/core';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';

@Component({
    selector: 'button-group',
    providers: [makeProvider(ButtonGroupComponent)],
    template: `<div class="btn btn-group btn-group-sm">
                    <button class="btn btn-secondary"
                            *ngIf="hasAnyOrAll"
                            [class.active]="isAnyOrAllChecked"
                            (click)="toggleAnyOrAll()">
                        {{anyOrAllText}}
                    </button>
                    <button class="btn btn-secondary"
                            *ngFor="let item of items"
                            [class.active]="isChecked(item)"
                            (click)="toggle(item)">
                        {{getItemLabel(item)}}
                    </button>
                </div>`
})
export class ButtonGroupComponent extends AbstractValueAccessor {
    @Input() items: any[];
    @Input() labelMember?: string;
    @Input() tooltipMember?: string; // todo: implement in template
    @Input() valueMember?: string;
    @Input() multipleSelect: boolean = false;
    @Input() anyOrAllText?: string;

    get hasAnyOrAll(): boolean {
        return !!this.anyOrAllText
            && this.anyOrAllText.length > 0;
    }

    get isAnyOrAllChecked(): boolean {
        return !this.selectedItemValue
            || this.selectedItemValue.length === 0;
    }

    private get selectedItemValue(): any | any[] {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        return this.value;
    }

    private set selectedItemValue(value: any | any[]) {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        this.value = value;
    }

    onValueChange(): void {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        if (this.multipleSelect && this.value === undefined) {
            this.value = [];
        }
    }

    toggleAnyOrAll() {
        if (!this.isAnyOrAllChecked) {
            this.selectedItemValue = this.multipleSelect ? [] : undefined;
        }
    }

    isChecked(item: any): boolean {
        let itemValue = this.getItemValue(item);
        return this.multipleSelect
            ? _.includes(this.selectedItemValue, itemValue)
            : this.selectedItemValue === itemValue;
    }

    toggle(item: any) {
        let itemValue = this.getItemValue(item);
        if (this.multipleSelect) {
            let newItemValues = [...this.selectedItemValue]; // create a copy
            let isChecked = _.includes(newItemValues, itemValue);
            if (isChecked) {
                _.remove(newItemValues, v => v === itemValue);
            }
            else {
                newItemValues.push(itemValue);
            }
            // to allow change detection to work as expected, we must overwrite
            // selectedItemValues rather than remove or insert an item directly
            this.selectedItemValue = newItemValues;
        }
        else {
            this.selectedItemValue = itemValue;
        }
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
