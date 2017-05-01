import { Routes, RouterModule } from '@angular/router';

export const routes: Routes = [
    { path: '', redirectTo: 'default', pathMatch: 'full' },
    {
        path: 'about', loadChildren: './about/about.module#AboutModule',
    }
];

export const AppRoutes = RouterModule.forRoot(routes);
