import { Routes } from '@angular/router';
import { LOGIN_ROUTES } from './pages/login/login.routes';
import { REGISTER_ROUTES } from './pages/register/register.routes';

export const routes: Routes = [...LOGIN_ROUTES, ...REGISTER_ROUTES];
