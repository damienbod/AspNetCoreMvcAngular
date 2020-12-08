import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AboutRoutes } from './about.routes';
import { AboutComponent } from './components/about.component';

@NgModule({
    imports: [
        CommonModule,
        AboutRoutes
    ],

    declarations: [
        AboutComponent
    ]
})

export class AboutModule { }
