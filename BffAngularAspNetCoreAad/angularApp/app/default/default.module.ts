import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { DefaultRoutes } from './default.routes';
import { DefaultComponent } from './components/default.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        DefaultRoutes
    ],

    declarations: [
        DefaultComponent
    ],

    exports: [
        DefaultComponent
    ]
})

export class DefaultModule { }
