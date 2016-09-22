export interface PbcatItemConfig {
    title: string;
    infoAttrValue: string;
    description?: string;
    subHeading?: string;

    // all but one screen have 0-1 image per item
    imageUrl?: string;

    // bike screen 6 has multiple images per item
    imageUrls?: string[];

    // the next screen may be fixed for a given item, or it may depend
    // on what option was selected on screen 1 (crash location)
    nextScreenName?: string | {[crashLocation: string]: string};
}

export interface PbcatScreenConfig {
    title: string;
    description: string;
    infoAttrName: string;
    items: PbcatItemConfig[];
}

export interface PbcatConfig {
    [screenName: string]: PbcatScreenConfig;
}
