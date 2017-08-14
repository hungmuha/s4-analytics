import { Component } from '@angular/core';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {
    collapseMap: boolean = false;
    collapseGrid: boolean = false;
    collapseCharts: boolean = false;

    toggleMap(): void {
        this.collapseMap = !this.collapseMap;
    }

    toggleGrid(): void {
        this.collapseGrid = !this.collapseGrid;
    }

    toggleCharts(): void {
        this.collapseCharts = !this.collapseCharts;
    }
}
