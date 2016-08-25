import { ParticipantType } from './pbcat.enums';

export interface PbcatItemConfig {
    title: string;
    infoAttrValue: number;
    description?: string;
    imageUrl?: string;
    nextScreenName?: string;
}

export interface PbcatScreenConfig {
    title: string;
    description: string;
    infoAttrName: string;
    items: PbcatItemConfig[];
}

export interface PbcatConfig {
    participantType: ParticipantType;
    [screenName: string]: PbcatScreenConfig;
}
