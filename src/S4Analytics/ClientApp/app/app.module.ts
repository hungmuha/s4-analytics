import { NgModule } from '@angular/core';
import { BrowserModule, __platform_browser_private__ } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { routing, appRoutingProviders } from './app.routing';
import { AppComponent } from './app.component';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { ReportViewerComponent } from './report-viewer.component';
import { PbcatModule } from './pbcat/pbcat.module';
import { OptionsService } from './options.service';
import { KeepSilverlightAliveService } from './keep-silverlight-alive.service';

@NgModule({
    imports: [
        BrowserModule,
        NgbModule.forRoot(),
        HttpModule,
        PbcatModule,
        routing
    ],
    declarations: [
        AppComponent,
        IndexComponent,
        LoginComponent,
        ReportViewerComponent
    ],
    providers: [
        appRoutingProviders,
        __platform_browser_private__.BROWSER_SANITIZATION_PROVIDERS,
        OptionsService,
        KeepSilverlightAliveService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
