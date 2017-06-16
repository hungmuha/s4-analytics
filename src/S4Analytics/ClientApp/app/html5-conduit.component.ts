import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({ template: '' })
export class Html5ConduitComponent {
    constructor(route: ActivatedRoute, router: Router) {
        route.params.subscribe(params => {
            console.log(params);
            let url = params['url'] as string;
            router.navigateByUrl(url);
        });
    }
}
