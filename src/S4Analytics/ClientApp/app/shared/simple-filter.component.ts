import { Component, Input } from '@angular/core';
import {  CheckableSettings, TreeItemLookup, TreeItem } from '@progress/kendo-angular-treeview';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';
import { Observable } from "rxjs/Observable";
import { FilterParams } from './filter-params';

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
            [nodes]= "formattedData"
            textField="text"
            kendoTreeViewExpandable
            [(checkedKeys)]="checkedKeys"
            [checkBy]="'id'"
            [isChecked]="isItemChecked"
            (checkedChange)="handleChecking($event)"
            [kendoTreeViewCheckable]="checkableSettings"
            >
         </kendo-treeview>
        </div>
      </div>`

})
export class SimpleFilterComponent extends AbstractValueAccessor{
    @Input() filterParams: FilterParams;

    filterName: string;
    checkMode?: 'single' | 'multiple';
    anyAllNone?: 'Any' | 'All';
    initialSelection: any[];
    defaultSelection: any[];
    nodes: any[];
    checkedIds: string[] = [];  // output for CrashQuery

    defaultCheckMode: any = 'single';
    collapseFilter1: boolean = false;

    public checkedKeys: any[];
    public checkedText: string[] = [];

    public formattedData: any[];

    public get checkableSettings(): CheckableSettings {
        return {
            checkChildren: false,
            checkParents: false,
            mode: this.checkMode ? this.checkMode : this.defaultCheckMode
        };
    }

    public ngOnInit(): void {

        this.filterName = this.filterParams.filterName;
        this.checkMode = this.filterParams.checkMode;
        this.anyAllNone = this.filterParams.anyOrAll;
        this.initialSelection = this.filterParams.initialSelection ? this.filterParams.initialSelection : [];
        // if no initial selection, use this.  when filter cleared, use this
        this.defaultSelection = this.initialSelection ?
            (this.filterParams.defaultSelection ?
                this.filterParams.defaultSelection
                : [])
            : [];

        this.nodes = this.filterParams.nodes;
        this.formattedData = this.getFormattedNodes();
        this.checkedKeys = this.initialSelection ? this.initialSelection : this.defaultSelection;
        //todo: set checkedtext from checked keys
    }

    toggleMoreFilterOptions() {
        this.collapseFilter1 = !this.collapseFilter1;

        if (!this.collapseFilter1) {
            // TODO: combine into one list of checked items
            this.checkedIds = [];
            this.checkedText = [];
            if (this.checkedKeys.length > 0) {
                // prepare choices for crash-query
                for (let key of this.checkedKeys) {
                    let item = _.find(this.formattedData, function (o) { return o.id === key; });

                    this.checkedIds.push(item.id);
                    this.checkedText.push(item.text);

                }
            }
        }
    }

    public isItemChecked = (_: any, index: string) => {
        return this.checkedKeys.indexOf(index) > -1 ? 'checked' : 'none';
    }


    //WIP
    public handleChecking(itemLookup: TreeItemLookup): void {
        //this.checkedKeys = [itemLookup.item];

        //if (this.isItemChecked(null, itemLookup.item.dataItem.id) === 'checked') {
        //    this.items.push(itemLookup.item);
        //}
    }

    getFormattedNodes(): any[] {
        let alphabeticalNodes = _.sortBy(this.nodes, [function (node: any) { return <string>node.text; }]);

        let formattedNodes = [];

        // add top node if needed
        if (this.anyAllNone) {
            formattedNodes.push({ id: this.anyAllNone, text: this.anyAllNone });
        }

        formattedNodes = formattedNodes.concat(alphabeticalNodes);
        return formattedNodes;
    }
}