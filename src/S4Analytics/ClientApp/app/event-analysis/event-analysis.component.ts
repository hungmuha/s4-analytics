import { Component } from '@angular/core';

@Component({
    templateUrl: './event-analysis.component.html'
})
export class EventAnalysisComponent {
    hideMap: boolean = false;
    hideGrid: boolean = false;
    hideCharts: boolean = false;

    toggleMap(): void {
        this.hideMap = !this.hideMap;
    }

    toggleGrid(): void {
        this.hideGrid = !this.hideGrid;
    }

    toggleCharts(): void {
        this.hideCharts = !this.hideCharts;
    }
}
