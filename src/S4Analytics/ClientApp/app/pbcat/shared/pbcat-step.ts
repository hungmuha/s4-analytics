import { PbcatItem } from './pbcat-item';

export class PbcatStep {
    items: PbcatItem[];
    private _selectedItem: PbcatItem;

    constructor(
        public screenName: string,
        public title: string,
        public description: string,
        public infoAttrName: string) { }

    get selectedItem(): PbcatItem {
        return this._selectedItem;
    }

    set selectedItem(value: PbcatItem) {
        this._selectedItem = value;
        for (let item of this.items) {
            item.selected = item === value;
        }
    }
}
