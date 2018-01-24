import { Component, Input } from '@angular/core';
import {  CheckableSettings, TreeItemLookup } from '@progress/kendo-angular-treeview';
import * as _ from 'lodash';
import { AbstractValueAccessor, makeProvider } from './abstract-value-accessor';
import { Observable } from "rxjs/Observable";

@Component({
    selector: `simple-filter`,
    providers: [makeProvider(SimpleFilterComponent)],
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
export class SimpleFilterComponent extends AbstractValueAccessor{
    @Input() filterName: string;
    @Input() checkParents: boolean;
    @Input() checkChildren: boolean;
    @Input() checkMode: any;
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
        this.formattedData = this.getFormattedNodes();
        this.checkedKeys = this.initialSelection ? this.initialSelection : this.defaultSelection;
    }

    toggleMoreFilterOptions() {
        this.collapseFilter1 = !this.collapseFilter1;
    }

    //WIP
    public handleChecking(itemLookup: TreeItemLookup): void {
        this.checkedKeys = [itemLookup.item.index];
    }

    getFormattedNodes(): any[] {
        let alphabeticalNodes = _.sortBy(this.nodes, [function (node: any) { return <string>node.text; }]);

        let formattedNodes = [];

        // add top node if needed
        if (this.anyAllNone) {
            formattedNodes.push({ text: this.anyAllNone });
        }

        formattedNodes = formattedNodes.concat(alphabeticalNodes);
        return formattedNodes;
    }
}