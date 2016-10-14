import { NgModule } from '@angular/core';
import { BrowserModule, __platform_browser_private__ } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { routing, appRoutingProviders } from './app.routing';
import { AppComponent } from './app.component';
import { IndexComponent } from './index.component';
import { LoginComponent } from './login.component';
import { ReportViewerComponent } from './report-viewer.component';
import { PbcatModule } from './pbcat/pbcat.module';
import { AppState } from './app.state';
import { OptionsService } from './options.service';
import { KeepSilverlightAliveService } from './keep-silverlight-alive.service';

@NgModule({
    imports: [
        BrowserModule,
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
        AppState,
        appRoutingProviders,
        __platform_browser_private__.BROWSER_SANITIZATION_PROVIDERS,
        OptionsService,
        KeepSilverlightAliveService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
