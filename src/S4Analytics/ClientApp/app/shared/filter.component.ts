﻿import { Component, Input } from '@angular/core';
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
            [nodes]= "nodes"
            textField="text"
            kendoTreeViewExpandable

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
    @Input() multilevel: boolean = false;
    @Input() anyAllNone?: string;
    @Input() initialSelection: any[];
    @Input() defaultSelection: any[] = this.defaultSelection ? this.defaultSelection : [];  // if no initial selection, use this.  when filter cleared, use this
    @Input() nodes: any[];

    defaultCheckMode: any = 'single';
    collapseFilter1: boolean = false;

    public checkedKeys: any[];

    public formattedData: any[];

    public get checkableSettings(): CheckableSettings {
        return {
            checkChildren: this.checkChildren,
            checkParents: this.checkParents,
            mode: this.checkMode ? this.checkMode : this.defaultCheckMode
        };
    }

    public ngOnInit(): void {
        if (this.multilevel) { console.log('multi'); } else { console.log('not multi');}

        this.formattedData = this.getFormattedNodes();
        this.checkedKeys = this.initialSelection ? this.initialSelection : this.defaultSelection;
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
        let selectedItem = itemLookup.item;
        let dataItem = itemLookup.item.dataItem;
        let isParent = itemLookup.parent?false:true


        console.log('add');

        this.checkedKeys = [itemLookup.item.index];


        //if (isParent) {
        //    console.log('deselect');
        //    this.checkedKeys = [itemLookup.item.index];
        //}

    }

    getFormattedNodes(): any[] {
        // Sort alphabetical
        let alphabeticalNodes = _.sortBy(this.nodes, [function (node: any) { return <string>node.text; }]);

        if (!this.multilevel) { return alphabeticalNodes; }

        let parentId = 0;
        let formattedNodes: any[] = [];

        // add top node if needed
        if (this.anyAllNone) {
            formattedNodes.push({ id: 0, text: this.anyAllNone });
        }

        for (let node of alphabeticalNodes) {
            let firstLetter = <string>node.text[0].charAt(0);

            let parentIndex = _.find(formattedNodes, function (node) { return node.text == firstLetter; });
            if (parentIndex == undefined) {
                parentId++;
                formattedNodes.push({ id: parentId, text: firstLetter });
            }
            formattedNodes.push({ id: node.id, parent: parentId, text: node.text });
        }

        return formattedNodes;
    }
}