import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    selector: 'admin',
    template: require('./admin.component.html')
})
export class AdminComponent {
    constructor(
        private router: Router
    ) { }
}
