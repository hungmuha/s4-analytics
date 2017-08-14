import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { NetworkAnalysisComponent } from './network-analysis.component';

@NgModule({
    imports: [
        RouterModule,
        CommonModule
    ],
    declarations: [
        NetworkAnalysisComponent
    ]
})
export class NetworkAnalysisModule { }
