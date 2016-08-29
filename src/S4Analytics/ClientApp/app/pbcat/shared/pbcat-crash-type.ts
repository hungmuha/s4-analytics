import { ParticipantType } from './pbcat.enums';

export class PbcatCrashType {
    participantType: ParticipantType;
    crashTypeNbr: number;
    crashTypeDesc: string;
    crashGroupNbr: number;
    crashGroupDesc: string;
    crashTypeExpanded: number;
    crashGroupExpanded: number;
}
