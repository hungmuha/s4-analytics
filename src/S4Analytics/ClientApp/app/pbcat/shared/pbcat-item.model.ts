export class PbcatItem {
    public notImplemented: boolean; // for temporary prototype use only

    constructor(
        public index: number,
        public title: string,
        public description: string = undefined,
        public imageUrl: string = undefined,
        public selected: boolean = false) {
        this.notImplemented = true;
    }
}
