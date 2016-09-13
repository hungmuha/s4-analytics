export class PbcatItem {
    selected: boolean = false;

    constructor(
        public index: number,
        public infoAttrValue: string,
        public title: string,
        public nextScreenName?: string,
        public description?: string,
        public subHeading?: string,
        public imageUrls?: string[]) { }
}
