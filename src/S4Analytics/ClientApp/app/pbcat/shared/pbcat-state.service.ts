import { Injectable } from '@angular/core';
import { PbcatFlow } from './pbcat-flow';
import { PbcatConfig } from './pbcat-config.d';

@Injectable()
export class PbcatStateService {
    autoAdvance: boolean = true;
    showReportViewer: boolean = false;
    reportViewerWindow: Window;
    nextHsmvNumber: number;
    config: PbcatConfig;
    flow: PbcatFlow;
    queue: number[];
}
