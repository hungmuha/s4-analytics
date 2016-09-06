import { Injectable } from '@angular/core';
import { PbcatState } from './pbcat/shared';

@Injectable()
export class AppState {
    public options: { [key: string]: any };
    private _pbcatState: PbcatState;

    constructor() {
        // todo: retrieve options from api
        this.options = {
            'silverlightBaseUrl': 'http://localhost:51063/'
        };
    }

    // read-only property for each module's state
    get pbcatState(): PbcatState {
        if (this._pbcatState === undefined) {
            this._pbcatState = new PbcatState();
        }
        return this._pbcatState;
    }
}
