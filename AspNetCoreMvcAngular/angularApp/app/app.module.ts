import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SharedModule } from './shared/shared.module';
import { CoreModule } from './core/core.module';
import { DefaultModule } from './default/default.module';

import { AppRoutes } from './app.routes';

import { AppComponent } from './app.component';

@NgModule({
    imports: [
        BrowserModule,
        AppRoutes,
        SharedModule,
        CoreModule.forRoot(),
        DefaultModule
    ],

    declarations: [
        AppComponent
    ],

    bootstrap: [AppComponent],
})

export class AppModule { }