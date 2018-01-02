import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GridModule } from '@progress/kendo-angular-grid';
import { routes } from './app.routing';
import { AdminModule } from './admin';
import { EventAnalysisModule } from './event-analysis';
import { NetworkAnalysisModule } from './network-analysis';
import { ReportingModule } from './reporting';
import { TrendAnalysisModule } from './trend-analysis';
import { AppComponent } from './app.component';
import { LoginComponent } from './login.component';
import { IndexComponent } from './index.component';
import { Html5ConduitComponent } from './html5-conduit.component';
import { PROVIDERS } from './shared';

@NgModule({
    imports: [
        RouterModule.forRoot(routes),
        HttpModule,
        BrowserModule,
        FormsModule,
        NgbModule.forRoot(),
        GridModule,
        AdminModule,
        EventAnalysisModule,
        NetworkAnalysisModule,
        ReportingModule,
        TrendAnalysisModule
    ],
    declarations: [
        AppComponent,
        IndexComponent,
        LoginComponent,
        Html5ConduitComponent
    ],
    providers: PROVIDERS,
    bootstrap: [AppComponent]
})
export class AppModule { }
