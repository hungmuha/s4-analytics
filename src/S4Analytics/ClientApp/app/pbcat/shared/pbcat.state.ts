import { FlowType, PbcatFlow } from './pbcat-flow.ts';
import { PbcatConfig } from './pbcat-config.d.ts';

export class PbcatState {
    autoAdvance: boolean = true;
    showReportViewer: boolean = false;
    reportViewerWindow: Window;
    nextFlowType: FlowType;
    nextHsmvNumber: number;
    config: PbcatConfig;
    flow: PbcatFlow;
}
