import { Routes } from '@angular/router';
import { LOGIN_ROUTES } from './pages/login/login.routes';

export const routes: Routes = [
    ...LOGIN_ROUTES,
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: '**', redirectTo: 'login' }
];
