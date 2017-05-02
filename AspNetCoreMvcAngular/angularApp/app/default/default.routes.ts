import { Routes, RouterModule } from '@angular/router';

import { DefaultComponent } from './components/default.component';

const routes: Routes = [
    { path: 'default', component: DefaultComponent }
];

export const DefaultRoutes = RouterModule.forChild(routes);