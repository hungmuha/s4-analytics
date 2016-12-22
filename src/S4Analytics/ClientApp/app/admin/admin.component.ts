import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
    template: require('./admin.component.html')
})
export class AdminComponent {
    private router: Router;
}