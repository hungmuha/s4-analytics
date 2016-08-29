import { PbcatConfig } from './pbcat-config.d.ts';
import { PbcatCrashType } from './pbcat-crash-type.ts';
import { PbcatFlow } from './pbcat-flow.ts';
import { PbcatInfo } from './pbcat-info.ts';

export class PbcatState {
    config: PbcatConfig;
    flow: PbcatFlow;
    info: PbcatInfo; // todo: move to flow?
    crashType: PbcatCrashType; // todo: move to flow?
}
