export class PbcatItem {
    constructor(
        public index: number,
        public title: string,
        public description: string = undefined,
        public imageUrl: string = undefined,
        public selected: boolean = false) { }
}
