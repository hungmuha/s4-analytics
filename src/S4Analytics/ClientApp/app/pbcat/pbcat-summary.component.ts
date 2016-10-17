import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PbcatStep, PbcatCrashType } from './shared';

@Component({
    selector: 'pbcat-summary',
    template: require('./pbcat-summary.component.html')
})
export class PbcatSummaryComponent {
    @Input() hsmvReportNumber: number;
    @Input() stepHistory: PbcatStep[];
    @Input() crashType: PbcatCrashType;
    @Input() isSaved: boolean;
    @Input() notes: string;
    @Output() notesChange = new EventEmitter<string>();
    @Output() jumpBackToStep = new EventEmitter<number>();

    private jumpBack(stepNumber: number) {
        if (!this.isSaved) {
            this.jumpBackToStep.emit(stepNumber);
        }
    }

    private changeNotes(notes: string) {
        if (!this.isSaved) {
            this.notes = notes;
            this.notesChange.emit(notes);
        }
    }

    private get ready() {
        return this.crashType !== undefined;
    }
}
