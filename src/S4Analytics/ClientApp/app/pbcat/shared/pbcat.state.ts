import { FlowType, PbcatFlow } from './pbcat-flow';
import { PbcatConfig } from './pbcat-config.d';

export class PbcatState {
    autoAdvance: boolean = true;
    showReportViewer: boolean = false;
    reportViewerWindow: Window;
    nextHsmvNumber: number;
    config: PbcatConfig;
    flow: PbcatFlow;
    queue: number[];
}
