import { Directive, OnDestroy, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TreeViewComponent, CheckableSettings, CheckedState, TreeItemLookup, TreeItem } from '@progress/kendo-angular-treeview';

import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/observable/merge';
import 'rxjs/add/operator/map';

const indexChecked = (keys:any[], index: string) => keys.filter(k => k === index).length > 0;

/**
 * A directive which manages the node in-memory checked state of the TreeView.
 * https://www.telerik.com/kendo-angular-ui-develop/components/treeview/checkboxes/
 */
@Directive({ selector: '[customCheck]' })
export class CustomCheckDirective implements OnInit, OnDestroy {
    /**
     * @hidden
     */
    @Input() public set isChecked(value: <T>(item: T, index: string) => CheckedState) {
        this.treeView.isChecked = value;
    }

    /**
     * Defines the collection that will store the checked keys.
     */
    @Input() public checkedKeys: any[] = [];

    /**
     * Fires when the `checkedKeys` collection was updated.
     */
    @Output() public checkedKeysChange: EventEmitter<string[]> = new EventEmitter<string[]>();

    protected subscriptions: Subscription = new Subscription(() => {/**/ });

    constructor(protected treeView: TreeViewComponent) { }

    public ngOnInit(): void {
        this.subscriptions.add(
            this.treeView.checkedChange
                .subscribe((e: TreeItemLookup) => this.checkMultiple(e))
        );

        this.treeView.checkboxes = true; //Shows checkboxes
        this.treeView.isChecked = this.isItemChecked.bind(this);
    }

    public ngOnDestroy(): void {
        this.subscriptions.unsubscribe();
    }

    protected isItemChecked(_: any, index: string): CheckedState {
        const checkedKeys = this.checkedKeys.filter((k) => k.indexOf(index) === 0);

        if (indexChecked(checkedKeys, index)) {
            return 'checked';
        }

        return 'none';
    }

    protected checkMultiple(node: TreeItemLookup): void {
        this.checkNode(node);
        this.checkParents(node.parent);
        this.notify();
    }

    private checkNode(node: TreeItemLookup, check?: boolean): void {
        const key = node.item.index;
        const idx = this.checkedKeys.indexOf(key);

        const isChecked = idx > -1;
        const shouldCheck = check === undefined ? !isChecked : check;
        const isKeyPresent = key !== undefined && key !== null;

        if (!isKeyPresent || (isChecked && check)) { return; }

        if (isChecked) {
            this.checkedKeys.splice(idx, 1);
        } else {
            this.checkedKeys.push(key);
        }
        if (node.children) {
            node.children.map(n => this.checkNode(n, shouldCheck));
        }

    }

    private checkParents(parent: any): void {
        let currentParent = parent;

        while (currentParent) {
            const parentKey = currentParent.item.index;
            const parentIndex = this.checkedKeys.indexOf(parentKey);

            if (this.allChildrenSelected(currentParent.children)) {
                if (parentIndex === -1) {
                    this.checkedKeys.push(parentKey);
                }
            } else if (parentIndex > -1) {
                this.checkedKeys.splice(parentIndex, 1);
            }

            currentParent = currentParent.parent;
        }
    }

    private allChildrenSelected(children: any[]): boolean {
        const isCheckedReducer = (checked: CheckedState, item: TreeItem) => (
            checked && this.isItemChecked(item.dataItem, item.index) === 'checked'
        );

        return children.reduce(isCheckedReducer, true);
    }

    private notify(): void {
        this.checkedKeysChange.emit(this.checkedKeys.slice());
    }
}

