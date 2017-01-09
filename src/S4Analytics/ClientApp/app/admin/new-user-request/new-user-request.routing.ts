import { Routes } from '@angular/router';
import { ContractViewerComponent } from './contract-viewer.component';


export const newUserRequestRoutes: Routes = [
    { path: 'contract-viewer/:requestNumber', component: ContractViewerComponent },
];
