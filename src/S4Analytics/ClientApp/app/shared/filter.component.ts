import { Component, Input } from '@angular/core';
import {  CheckableSettings, TreeItemLookup } from '@progress/kendo-angular-treeview';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';
import { Observable } from "rxjs/Observable";

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
        <kendo-treeview
            [nodes]= "formattedData"
            textField="text"
            kendoTreeViewExpandable
            kendoTreeViewFlatDataBinding
            [(checkedKeys)]="checkedKeys"
            [checkBy]="'text'"
            [isChecked]="isItemChecked"
            (checkedChange)="handleChecking($event)"
            [kendoTreeViewCheckable]="checkableSettings"
            idField="id"
            parentIdField="parent"
            >
         </kendo-treeview>
        </div>
      </div>`

})
export class FilterComponent extends AbstractValueAccessor{
    @Input() filterName: string;
    @Input() checkParents: boolean;
    @Input() checkChildren: boolean;
    @Input() checkMode: any;
    @Input() isMultilevel: boolean = this.isMultilevel ? this.isMultilevel : true;  // default = false
    @Input() anyAllNone?: any[];
    @Input() initialSelection: any[] = this.initialSelection ? this.initialSelection : [];
    @Input() defaultSelection: any[] = this.defaultSelection ? this.defaultSelection : [];  // if no initial selection, use this.  when filter cleared, use this
    @Input() sort: boolean = true;  // default = true
    @Input() nodes: any[];

    // if multilevel = true, must be sort = true
    defaultCheckChildren: boolean = false;
    defaultCheckParents: boolean = false;
    defaultCheckMode: any = 'multiple'; // default = single
    collapseFilter1: boolean = false;

    checkedIndices: string[] = ['0'];
    headerIndices: string[] = [];

    public checkedKeys: any[] = this.initialSelection ? this.initialSelection : this.defaultSelection;

    public formattedData: any[];

    public get checkableSettings(): CheckableSettings {
        return {
            checkChildren: this.checkChildren ? this.checkChildren : this.checkChildren,
            checkParents: this.checkParents ? this.checkParents : this.checkParents,
            mode: this.checkMode ? this.checkMode : this.defaultCheckMode
        };
    }

    public ngOnInit(): void {
        this.formattedData = this.getFormattedNodes();
    }

    public isItemChecked = (_: any, index: string) => {
        return this.checkedKeys.indexOf(index) > -1 ? 'checked' : 'none';
    }

    toggleMoreFilterOptions() {
        this.collapseFilter1 = !this.collapseFilter1;
    }

    //WIP
    public handleChecking(itemLookup: TreeItemLookup): void {
        console.log('handle checking');
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

    getFormattedNodes(): any[] {
        if (!this.sort) { return this.nodes; }
        // Sort alphabetical
        let alphabeticalNodes = _.sortBy(this.nodes, [function (node: any) { return <string>node.text; }]);
        if (!this.isMultilevel) { return alphabeticalNodes; }

        let parentId = 0;
        let formattedNodes: any[] = [];

        for (let node of alphabeticalNodes) {
            let firstLetter = <string>node.text[0].charAt(0);

            let parentIndex = _.find(formattedNodes, function (node) { return node.text == firstLetter; });
            if (parentIndex == undefined) {
                parentId++;
                formattedNodes.push({ id: parentId, text: firstLetter });
            }
            formattedNodes.push({ id: node.id, parent: parentId, text: node.text });
        }

        if (this.anyAllNone) {
            // add top 'Any' 'All' row

        }

        return formattedNodes;
    }

}