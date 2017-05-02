import { Component, OnInit } from '@angular/core';
// import { CrashService, CrashQuery } from './shared';

export class Page {
    //The number of elements in the page
    size: number = 0;
    //The total number of elements
    totalElements: number = 0;
    //The total number of pages
    totalPages: number = 0;
    //The current page number
    pageNumber: number = 0;
}

export class PagedData<T> {
    data = new Array<T>();
    page = new Page();
}

@Component({
    templateUrl: './ngx-datatable-poc.component.html'
})
export class NgxDatatablePocComponent implements OnInit {
    rows = [
        { name: 'Austin', gender: 'Male', company: 'Swimlane' },
        { name: 'Dany', gender: 'Male', company: 'KFC' },
        { name: 'Molly', gender: 'Female', company: 'Burger King' },
    ];
    columns = [
        { name: 'Name' },
        { name: 'Gender' },
        { name: 'Company' }
    ];

    ngOnInit() { }
}
