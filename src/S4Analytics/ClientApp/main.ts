import 'zone.js';
import 'reflect-metadata';
import './site.css';

// Import the browser platform with a compiler
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

// Import the app module
import { AppModule } from './app/app.module';

// Compile and launch the app module
platformBrowserDynamic().bootstrapModule(AppModule);

// Basic hot reloading support. Automatically reloads and restarts the Angular 2 app each time
// you modify source files. This will not preserve any application state other than the URL.
declare var module: any;
if (module.hot) {
    module.hot.accept();
}
