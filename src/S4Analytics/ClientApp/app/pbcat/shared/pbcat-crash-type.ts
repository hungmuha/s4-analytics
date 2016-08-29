import { FlowType } from './pbcat.state';

export class PbcatCrashType {
    flowType: FlowType;
    crashTypeNbr: number;
    crashTypeDesc: string;
    crashGroupNbr: number;
    crashGroupDesc: string;
    crashTypeExpanded: number;
    crashGroupExpanded: number;
}
