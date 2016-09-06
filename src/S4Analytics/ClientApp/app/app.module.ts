import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { routing, appRoutingProviders } from './app.routing';
import { AppComponent } from './app.component';
import { PbcatModule } from './pbcat/pbcat.module';
import { AppState } from './app.state';

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        PbcatModule,
        routing
    ],
    declarations: [AppComponent],
    providers: [AppState, appRoutingProviders],
    bootstrap: [AppComponent]
})
export class AppModule { }
