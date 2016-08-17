import { PbcatItem } from './pbcat-item.model';

export class PbcatStep {
    items: PbcatItem[];

    constructor(
        public index: number,
        public title: string,
        public infoAttr: string,
        public description: string = undefined) { }
}
