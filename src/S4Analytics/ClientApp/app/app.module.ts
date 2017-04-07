import { NgModule } from '@angular/core';
import { BrowserModule, __platform_browser_private__ } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { routes } from './app.routing';
import { AdminModule } from './admin/admin.module';
import { EventAnalysisModule } from './event-analysis/event-analysis.module';
import { NetworkAnalysisModule } from './network-analysis/network-analysis.module';
import { ReportingModule } from './reporting/reporting.module';
import { TrendAnalysisModule } from './trend-analysis/trend-analysis.module';
import { AppComponent } from './app.component';
import { IndexComponent } from './index.component';
import { AnalyticsComponent } from './analytics.component';
import { LoginComponent } from './login.component';
import { OptionsService } from './options.service';
import { KeepSilverlightAliveService } from './keep-silverlight-alive.service';

@NgModule({
    imports: [
        RouterModule.forRoot(routes),
        HttpModule,
        BrowserModule,
        NgbModule.forRoot(),
        AdminModule,
        EventAnalysisModule,
        NetworkAnalysisModule,
        ReportingModule,
        TrendAnalysisModule
    ],
    declarations: [
        AppComponent,
        IndexComponent,
        AnalyticsComponent,
        LoginComponent
    ],
    providers: [
        __platform_browser_private__.BROWSER_SANITIZATION_PROVIDERS,
        OptionsService,
        KeepSilverlightAliveService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
