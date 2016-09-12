export class PbcatItem {
    constructor(
        public index: number,
        public enumValue: string,
        public title: string,
        public nextScreenName?: string,
        public description?: string,
        public subHeading?: string,
        public imageUrls?: string[],
        public selected?: boolean) { }
}
