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
            [nodes]= "formattedNodes"
            textField="text"
            kendoTreeViewExpandable
            [(checkedKeys)]="checkedKeys"
            [kendoTreeViewCheckable]="checkableSettings"
            [checkBy]="'text'"
            [isChecked]="isItemChecked"
            (checkedChange)="onValueChanged($event)"

         >
         </kendo-treeview>
        </div>
      </div>`

})
export class SimpleFilterComponent extends AbstractValueAccessor {
    @Input() filterName: string;
    @Input() checkMode: 'multiple'|'single';
    @Input() nodes: any[];
    @Input() anyOrAllText?: string;
    @Input() valueMember?: string;

    public checkedText: string[] = [];
    checkedKeys = [];
    defaultCheckMode: 'multiple' | 'single' = 'single';
    collapseFilter1: boolean = false;

    public get formattedNodes(): any[] {
        let alphabeticalNodes = _.sortBy(this.nodes, [function (node: any) { return node.text as string; }]);

        let formattedNodes = [];

        // add top node if needed
        if (this.anyOrAllText) {
            formattedNodes.push({ id: this.anyOrAllText, text: this.anyOrAllText });
        }

        formattedNodes = formattedNodes.concat(alphabeticalNodes);
        return formattedNodes;
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

    public onValueChanged(itemLookup: TreeItemLookup): void {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        if (this.multipleSelect && this.value === undefined) {
            this.value = [];
        }
        this.toggle(itemLookup.item.dataItem.id);
    }

    // todo
    toggleAnyOrAll() {
        if (!this.isAnyOrAllChecked) {
            this.selectedItemValue = this.multipleSelect ? [] : undefined;
        }
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

    public isItemChecked = (item: string) => {
        console.log('is item checked');
        let itemValue = this.getItemValue(item);
        return this.multipleSelect
            ? _.includes(this.selectedItemValue, itemValue)
            : this.selectedItemValue === itemValue;
    }

    toggleMoreFilterOptions() {
        this.collapseFilter1 = !this.collapseFilter1;

        if (!this.collapseFilter1) {
            this.checkedText = [];
            if (this.value.length > 0) {
                for (let key of this.value) {
                    let item = _.find(this.nodes, function (o) { return o.id === key; });
                    this.checkedText.push(item.text);
                }
            }
        }
    }

    private getItemValue(item: any): any {
        let value = this.valueMember !== undefined
            ? item[this.valueMember]
            : item;
        return value;
    }
}