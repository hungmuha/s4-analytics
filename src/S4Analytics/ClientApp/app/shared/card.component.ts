import { Component, Input } from '@angular/core';

@Component({
    selector: 'card',
    template: `<div class="card mb-3" [class.collapsible]="collapsible" [class.collapsed]="collapsed">
        <div class="card-header d-flex justify-content-between align-content-center" (click)="toggle()" *ngIf="!hideHeader">
            <div>
                <ng-content select="[card-header]"></ng-content>
            </div>
            <span class="fa"
                    [class.fa-angle-left]="collapsed"
                    [class.fa-angle-down]="!collapsed"
                    *ngIf="collapsible"></span>
        </div>
        <div class="card-block">
            <ng-content select="[card-block]"></ng-content>
        </div>
        <div class="card-footer d-flex justify-content-between align-content-center" *ngIf="!hideFooter">
            <ng-content select="[card-footer]"></ng-content>
        </div>
    </div>`
})
export class CardComponent {
    private _collapsed: boolean;

    @Input() hideHeader: boolean;
    @Input() hideFooter: boolean;
    @Input() collapsible: boolean = true;
    @Input() set collapsed(value: boolean) {
        this._collapsed = this.collapsible ? value : false;
    };
    get collapsed(): boolean {
        return this._collapsed;
    }

    toggle() {
        if (this.collapsible) {
            this.collapsed = !this.collapsed;
        }
    }
}
