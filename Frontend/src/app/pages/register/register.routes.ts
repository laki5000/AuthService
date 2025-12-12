import { Routes } from '@angular/router';
import { RegisterComponent } from './register.component';
import { RouteConstants } from '../../core/constants/route.constant';

export const REGISTER_ROUTES: Routes = [
  {
    path: RouteConstants.Register,
    component: RegisterComponent,
  },
];
