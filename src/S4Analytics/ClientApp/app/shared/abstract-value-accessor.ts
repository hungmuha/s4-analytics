// See http://stackoverflow.com/a/37786142/204900

import { forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export abstract class AbstractValueAccessor implements ControlValueAccessor {

    _value: any = '';

    get value(): any { return this._value; }

    set value(v: any) {
        if (v !== this._value) {
            this._value = v;
            this.onValueChange(v);
            this.onChange(v);
        }
    }

    writeValue(value: any) {
        this._value = value;
        this.onValueChange(value);
        this.onChange(value);
    }

    // to be overridden by subclass for value change notifications
    onValueChange(_: any) { }

    // angular value change notifications (not used by subclass)
    onChange = (_: any) => { };
    onTouched = () => { };
    registerOnChange(fn: (_: any) => void): void { this.onChange = fn; }
    registerOnTouched(fn: () => void): void { this.onTouched = fn; }
}

export function makeProvider(type: any) {
    return {
        provide: NG_VALUE_ACCESSOR,
        useExisting: forwardRef(() => type),
        multi: true
    };
}
