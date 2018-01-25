import { Component, Input } from '@angular/core';
import {  CheckableSettings, TreeItemLookup } from '@progress/kendo-angular-treeview';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';

@Component({
    selector: `simple-filter`,
    providers: [makeProvider(SimpleFilterComponent)],
    template:
    `<div class="card card-inverse bg-inverse collapsible" [class.collapsed]="!collapseFilter1">
        <div class="card-header" (click)="toggleMoreFilterOptions()">
           {{filterName}}</div>
        <div *ngIf="!collapseFilter1">
          <ul *ngFor="let selection of checkedText">
            <li>{{selection}}</li>
          </ul>
        </div>

       <div class="card-block">
        <kendo-treeview
            *ngIf="hasAnyOrAll"
            [nodes]="[{id: 'All', text: 'All'}]"
            textField="text"
            [kendoTreeViewCheckable]="checkableSettings"
            [isChecked]="isAllItemChecked"
            (checkedChange)="onAllValueChanged($event)"
        >
        </kendo-treeview>
        <kendo-treeview
            [nodes]= "nodes"
            textField="text"
            kendoTreeViewExpandable

            [kendoTreeViewCheckable]="{ mode: checkMode }"

            [isChecked]="isItemChecked"
            (checkedChange)="onValueChanged($event)"
         >
         </kendo-treeview>
        </div>
      </div>`

})

    //          [checkBy]="'text'"
export class SimpleFilterComponent extends AbstractValueAccessor {
    @Input() filterName: string;
    @Input() checkMode: 'multiple'|'single';
    @Input() nodes: any[];
    @Input() anyOrAllText?: string;

    public checkedText: string[] = []; //should be able to use checkedKeys??
    public checkedKeys:any[] = [];
    defaultCheckMode: 'multiple' | 'single' = 'single';
    collapseFilter1: boolean = false;

    public isItemChecked = (_: any, item: string) => {
        return this.multipleSelect
            ? _.includes(this.selectedItemValue, item) ? 'checked' : 'none'
            : this.selectedItemValue === item ? 'checked' : 'none';
    }

    public isAllItemChecked = (_: any, item: string) => {
        return this.multipleSelect
            ? _.includes(this.selectedItemValue, item) ? 'checked' : 'none'
            : this.selectedItemValue === item ? 'checked' : 'none';
    }


    public get checkableSettings(): CheckableSettings {
        return {
            checkChildren: false,
            checkParents: false,
            mode: this.checkMode ? this.checkMode : this.defaultCheckMode
        };
    }

    get multipleSelect(): boolean {
        return this.checkMode === 'multiple';
    }

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

    public onAllValueChanged(itemLookup: TreeItemLookup): void {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        if (this.multipleSelect && this.value === undefined) {
            this.value = [];
        }

         this.toggleAnyOrAll(itemLookup.item.dataItem.id);
    }

    public onValueChanged(itemLookup: TreeItemLookup): void {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        if (this.multipleSelect && this.value === undefined) {
            this.value = [];
        }

        this.checkedKeys = [itemLookup.item.index];
        this.toggle(itemLookup.item.dataItem.id);
    }

    // todo need to clear checkboxes
    toggleAnyOrAll(itemLookup: TreeItemLookup) {
        if (!this.isAnyOrAllChecked) {
            this.selectedItemValue = this.multipleSelect ? [] : undefined;
            // clear check boxes

        }
    }

    // todo: need to uncheck ALL
    toggle(item: any) {
        if (this.multipleSelect) {
            let newItemValues = [...this.selectedItemValue]; // create a copy
            let isChecked = _.includes(newItemValues, item);
            if (isChecked) {
                _.remove(newItemValues, v => v === item);
            }
            else {
                newItemValues.push(item);
            }
            // to allow change detection to work as expected, we must overwrite
            // selectedItemValues rather than remove or insert an item directly
            this.selectedItemValue = newItemValues;
        }
        else {
            this.selectedItemValue = item;
        }
    }

    toggleMoreFilterOptions() {
        this.collapseFilter1 = !this.collapseFilter1;

        if (!this.collapseFilter1) {
            this.checkedText = [];
            if (this.value && this.value.length > 0) {
                for (let key of this.value) {
                    let item = _.find(this.nodes, function (o) { return o.id === key; });
                    this.checkedText.push(item.text);
                }
            }
        }
    }

}