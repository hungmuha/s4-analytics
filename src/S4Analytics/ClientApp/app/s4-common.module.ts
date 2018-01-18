import { NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GridModule } from '@progress/kendo-angular-grid';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { COMPONENTS, PROVIDERS } from './shared';

@NgModule({
    imports: [
        CommonModule,
        NgbModule
    ],
    declarations: [
        ...COMPONENTS
    ],
    providers: PROVIDERS,
    exports: [
        CommonModule,
        HttpModule,
        RouterModule,
        FormsModule,
        DatePipe,
        NgbModule,
        GridModule,
        DatePickerModule,
        BrowserAnimationsModule,
        ...COMPONENTS
    ]
})
export class S4CommonModule { }
