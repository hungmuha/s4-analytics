export class PbcatItem {
    public notImplemented: boolean; // for temporary prototype use only

    constructor(
        public index: number,
        public infoAttrValue: any,
        public title: string,
        public nextScreenName?: string,
        public description?: string,
        public imageUrl?: string,
        public selected?: boolean) {
        this.notImplemented = nextScreenName === undefined;
    }
}
