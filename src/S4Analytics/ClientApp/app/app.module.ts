import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { routes } from './app.routing';
import { S4CommonModule } from './s4-common.module';
import { AdminModule } from './admin';
import { EventAnalysisModule } from './event-analysis';
import { ReportingModule } from './reporting';
import { AppComponent } from './app.component';
import { LoginComponent } from './login.component';
import { IndexComponent } from './index.component';
import { Html5ConduitComponent } from './html5-conduit.component';


@NgModule({
    imports: [
        RouterModule.forRoot(routes),
        NgbModule.forRoot(),
        S4CommonModule,
        AdminModule,
        EventAnalysisModule,
        ReportingModule
    ],
    declarations: [
        AppComponent,
        IndexComponent,
        LoginComponent,
        Html5ConduitComponent

    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class AppModule { }
