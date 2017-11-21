import { Component, Input } from '@angular/core';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';

@Component({
    selector: 'radio-button-group',
    providers: [makeProvider(RadioButtonGroupComponent)],
    template: `<fieldset class="form-group">
                    <h6><b>{{groupLabel}}</b></h6>
                    <div class="form-check" [class.form-check-inline]="inline" *ngFor="let item of items">
                        <label class="form-check-label">
                            <input type="radio"
                                    class="form-check-input"
                                    [name]="name"
                                    [checked]="isChecked(item)"
                                    (click)="select(item)" />
                            <span ngbTooltip="{{getItemTooltip(item)}}" container="body" placement="right">
                                {{getItemLabel(item)}}
                            </span>
                        </label>
                    </div>
                </fieldset>`
})
export class RadioButtonGroupComponent extends AbstractValueAccessor {
    @Input() name: string;
    @Input() groupLabel: string;
    @Input() items: any[];
    @Input() labelMember?: string;
    @Input() tooltipMember?: string;
    @Input() valueMember?: string;
    @Input() inline?: boolean = true;

    private get selectedItemValue(): any {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        return this.value;
    }

    private set selectedItemValue(value: any) {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        this.value = value;
    }

    isChecked(item: any): boolean {
        let itemValue = this.getItemValue(item);
        return this.selectedItemValue === itemValue;
    }

    select(item: any) {
        let itemValue = this.getItemValue(item);
        this.selectedItemValue = itemValue;
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
