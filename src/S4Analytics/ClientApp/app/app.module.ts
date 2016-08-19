import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule, JsonpModule } from '@angular/http';
import { routing, appRoutingProviders } from './app.routing';
import { AppComponent } from './app.component';
import { PbcatModule } from './pbcat/pbcat.module';

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        JsonpModule,
        PbcatModule,
        routing
    ],
    declarations: [AppComponent],
    providers: [appRoutingProviders],
    bootstrap: [AppComponent]
})
export class AppModule { }
