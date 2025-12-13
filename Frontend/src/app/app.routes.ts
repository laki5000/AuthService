import { Routes } from '@angular/router';
import { LOGIN_ROUTES } from './pages/login/login.routes';
import { REGISTER_ROUTES } from './pages/register/register.routes';
import { DASHBOARD_ROUTES } from './pages/dashboard/dashboard.routes';
import { RouteConstants } from './core/constants/route.constant';

export const routes: Routes = [
  ...LOGIN_ROUTES,
  ...REGISTER_ROUTES,
  ...DASHBOARD_ROUTES,

  { path: '', redirectTo: RouteConstants.DASHBOARD_PATH, pathMatch: 'full' },
  { path: '**', redirectTo: RouteConstants.DASHBOARD_PATH, pathMatch: 'full' },
];
