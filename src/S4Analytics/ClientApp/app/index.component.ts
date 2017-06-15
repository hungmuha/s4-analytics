import { Component } from '@angular/core';
import { Http } from '@angular/http';
import { Router } from '@angular/router';

@Component({
    selector: 'index',
    templateUrl: './index.component.html'
})
export class IndexComponent {

    constructor(
        private http: Http,
        private router: Router) { }

    logOut(): void {
        this.http
            .post('api/logout', {})
            .subscribe(() => {
                this.router.navigate(['', 'login']);
            });
    }
}
