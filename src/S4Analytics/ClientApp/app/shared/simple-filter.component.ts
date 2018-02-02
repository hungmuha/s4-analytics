import { Component, Input } from '@angular/core';
import { TreeItemLookup } from '@progress/kendo-angular-treeview';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';

@Component({
    selector: `simple-filter`,
    providers: [makeProvider(SimpleFilterComponent)],
    template: `
    <filter-card [selectedText]="selectedText">
        <ng-container card-header>{{filterName}}</ng-container>
        <div card-block>
           <kendo-treeview
                [nodes]= "test"
                textField="name"
                [children]="children"
                [hasChildren]="hasChildren"
                customCheck
                [(checkedKeys)]="checkedKeys"
                (checkedChange)="onValueChanged($event)"
                >
             </kendo-treeview>
        </div>

      </filter-card>`
})

export class SimpleFilterComponent extends AbstractValueAccessor {
    @Input() filterName: string;
    @Input() checkMode: 'multiple' | 'single';
    @Input() nodes: any[];
    @Input() anyOrAllText?: string;

    // temporary for testing
    public test: any[] = [
        {
            name: 'Any', items: [
                { key: 'OffRoadAll', name: 'Off Road - All' },
                { key: 'OffRoadRollover', name: 'Off Road - Rollover' },
                { key: 'OffRoadCollision', name: 'Off Road - Collision Fixed Object' },
                { key: 'CrossedIntoTraffic', name: 'Crossed into Oncoming Traffic' },
                { key: 'Sideswipe', name: 'Sideswipe' }
            ]
        }];

    public checkedKeys: string[] = [];
    public selectedText: string[] = [];

    private get selectedItemValue(): any | any[] {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.

        return this.value;
    }

    private set selectedItemValue(value: any | any[]) {
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        this.value = value;
    }

    public children = (dataItem: any): Observable<any[]> => of(dataItem.items);
    public hasChildren = (dataItem: any): boolean => !!dataItem.items;

    get multipleSelect(): boolean {
        return this.checkMode === 'multiple';
    }

    // WIP: won't allow this to be checked when done this way...
    //  public anyOrAll:any[] = [{ key: -1, name: (this.anyOrAllText) ? this.anyOrAllText: '' }];

    public onValueChanged(itemLookup: TreeItemLookup): void {
        console.log('onvaluechanged ' + itemLookup.item.dataItem.key);
        // `this.value` maps to `ngModel` and is provided by the `AbstractValueAccessor` base class.
        if (this.multipleSelect && this.value === undefined) {
            this.value = [];
        }

        this.checkedKeys = [itemLookup.item.index];
        this.toggle(itemLookup.item.dataItem.key);
    }

    toggle(selectedItem: any) {
        if (!selectedItem) {
            // Any/All has been selected
            this.selectedItemValue = this.multipleSelect ? [] : undefined;
            this.selectedText = ['All'];  // need to use AnyOrAllText
            return;
        }

        if (this.multipleSelect) {
            let newItemValues = [...this.selectedItemValue]; // create a copy
            let isChecked = _.includes(newItemValues, selectedItem);
            if (isChecked) {
                _.remove(newItemValues, v => v === selectedItem);
            }
            else {
                newItemValues.push(selectedItem);
            }
            // to allow change detection to work as expected, we must overwrite
            // selectedItemValues rather than remove or insert an item directly
            this.selectedItemValue = newItemValues;
            this.selectedText = [];

            for (let key of newItemValues) {
                let item = _.find(this.nodes[0].items, function (o) { return o.key === key; });
                if (item) {
                    this.selectedText.push(item.name);
                }
            }
        }
        else {
            this.selectedItemValue = selectedItem;
            this.selectedText = [selectedItem];
        }


    }
}