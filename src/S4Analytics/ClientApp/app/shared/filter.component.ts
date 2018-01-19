import { Component, Input } from '@angular/core';
import {  CheckableSettings, TreeItemLookup } from '@progress/kendo-angular-treeview';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';

//import { S4FilterParams } from "app/ s4filter-params";

@Component({
    selector: `filter`,
    providers: [makeProvider(FilterComponent)],
    template:
    `<div class="card card-inverse bg-inverse collapsible" [class.collapsed]="!collapseFilter1">
        <div class="card-header" (click)="toggleMoreFilterOptions()">{{filterName}}</div>
        <div *ngIf="!collapseFilter1">
          <div *ngFor="let selection of checkedKeys">
            *{{ selection }}
          </div>
        </div>

       <div class="card-block">
        <kendo-treeview [nodes]= "nodes"
                          textField="text"
                          kendoTreeViewExpandable
                          kendoTreeViewFlatDataBinding
                          [(checkedKeys)]="checkedKeys"
                          [checkBy]="'text'"
                          [isChecked]="isItemChecked"
                          (checkedChange)="handleChecking($event)"
                          [kendoTreeViewCheckable]="checkableSettings"
                          idField="id"
                          parentIdField="parentId">
          </kendo-treeview>
        </div>
      </div>`

})
export class FilterComponent extends AbstractValueAccessor{
    @Input() filterName: string =  'TESTING';
    @Input() nodes: any[] = [{ id: 0, text: 'All' }, { id: 1, text: "A" },{ id: 11, parentId: 1, text: "Alachua" }];
    @Input() checkParents: boolean;
    @Input() checkChildren: boolean;
    @Input() checkMode: any;
    @Input() isMultilevel: boolean = this.isMultilevel ? this.isMultilevel : false;
    @Input() anyAllNone?: any[];
    @Input() initialSelection: any[] = this.initialSelection ? this.initialSelection : [];
    @Input() defaultSelection: any[] = this.defaultSelection ? this.defaultSelection : [];  // if no initial selection, use this.  when filter cleared, use this
    @Input() sort: boolean;

    // if multilevel = true, must be sort = true

    defaultCheckChildren: boolean = false;
    defaultCheckParents: boolean = false;
    defaultCheckMode: any = 'single';
    collapseFilter1: boolean = false;

    checkedIndices: string[] = [];
    headerIndices: string[] = [];

    public checkedKeys: any[] = this.initialSelection ? this.initialSelection : this.defaultSelection;

    public get checkableSettings(): CheckableSettings {
        return {
            checkChildren: this.checkChildren ? this.checkChildren : this.checkChildren,
            checkParents: this.checkParents ? this.checkParents : this.checkParents,
            mode: this.checkMode ? this.checkMode : this.defaultCheckMode
        };
    }

    public isItemChecked = (_: any, index: string) => {
        return this.checkedKeys.indexOf(index) > -1 ? 'checked' : 'none';
    }

    toggleMoreFilterOptions() {
        this.collapseFilter1 = !this.collapseFilter1;
    }

    //WIP
    public handleChecking(itemLookup: TreeItemLookup): void {
        let selectedIndex = itemLookup.item.index;

        if (this.checkedIndices.indexOf(selectedIndex) > -1) {
            console.log('deselect');
            // item has been deselected
            return;
        }

        //todo:  create based on how many parents if multilevel
        if (this.isMultilevel) {
            if (this.headerIndices.indexOf(selectedIndex) > -1) {
                console.log('clear');
                // uncheck item
                //???
                //remove index ie '2_1''
                // this.checkedIndices ....
                return;
            }
        }

        console.log('add');

        // add item name ie 'Alachua'
        this.checkedKeys = [itemLookup.item.index];

        //add index ie '2_1''
        // this.checkedIndices.push(selectedIndex);
    }

    //formattedNodes(): any[] {
    //    if (!this.sort) { return this.nodes; }

    //    // Sort alphabetical
    //    let alphabeticalNodes = _.sortBy(this.nodes, [function (node) { return node.text; }]);

    //    if (!this.isMultilevel) { return alphabeticalNodes; }

    //    let headerIndex = this.anyAllNone ? 1 : 0;
    //    let currentHeaderValue = alphabeticalNodes[0].charAt(0);

    //    let formattedNodes: any[] = [];  // add in first header row
    //    let header: string;

    //    for (let node of alphabeticalNodes) {
    //        let headerValue = node.text[0].charAt(0);
    //        if (headerValue !== currentHeaderValue) {
    //            // add in new header row to formatedNodes (headerIndex, headerValue)

    //            currentHeaderValue = headerValue;
    //            headerIndex++;
    //            // add string of headerIndex to end of headerIndices
    //        }
    //        // add in new node row
    //    }

    //    if (this.anyAllNone) {
    //        // add top 'Any' 'All' row

    //    }

    //    return formattedNodes;

    //}
}