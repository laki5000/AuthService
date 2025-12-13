import { Routes } from '@angular/router';
import { LoginComponent } from './login.component';
import { RouteConstants } from '../../core/constants/route.constant';

export const LOGIN_ROUTES: Routes = [
  {
    path: RouteConstants.LOGIN,
    component: LoginComponent,
  },
];
