import * as ng from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';

@ng.Component({
    selector: 'app',
    template: require('./app.html'),
    directives: ROUTER_DIRECTIVES
})
export class App {
}
