import { Component, Input } from '@angular/core';

// see https://scotch.io/tutorials/angular-2-transclusion-using-ng-content

@Component({
    selector: 'filter-card',
    template:
    `<div class="card mb-3" [class.collapsible]="collapsible" [class.collapsed]="collapsed">
        <div class="card-header d-flex justify-content-between" (click)="toggle()" *ngIf="!hideHeader">
            <ng-content select="[card-header]"></ng-content>
            <span class="fa"
                  [class.fa-angle-left]="collapsed"
                  [class.fa-angle-down]="!collapsed"
                  *ngIf="collapsible"></span>
        </div>
         <div *ngIf="collapsed">
            <ul *ngFor="let selection of selectedText">
                <li>{{selection}}</li>
            </ul>
        </div>
        <div class="card-block">
            <ng-content select="[card-block]"></ng-content>
        </div>
        <div class="card-footer d-flex justify-content-between" *ngIf="!hideFooter">
            <ng-content select="[card-footer]"></ng-content>
        </div>
    </div>`
})
export class FilterCardComponent {
    @Input() hideHeader: boolean;
    @Input() hideFooter: boolean;
    @Input() collapsible: boolean = true;
    @Input() selectedText: string[];
    @Input() set collapsed(value: boolean) {
        this._collapsed = this.collapsible ? value : false;
    }
    get collapsed(): boolean {
        return this._collapsed;
    }

    private _collapsed: boolean = true;

    toggle() {
        if (this.collapsible) {
            this.collapsed = !this.collapsed;
        }
    }
}
