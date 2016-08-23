import { PbcatItem } from './pbcat-item.model';

export class PbcatStep {
    items: PbcatItem[];
    selectedItem: PbcatItem;

    constructor(
        public title: string,
        public infoAttrName: string) { }
}
