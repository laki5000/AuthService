import { Routes } from '@angular/router';
import { DashboardComponent } from './dashboard.component';
import { AuthGuard } from '../../core/guards/auth.guard';
import { RouteConstants } from '../../core/constants/route.constant';

export const DASHBOARD_ROUTES: Routes = [
  {
    path: RouteConstants.DASHBOARD,
    component: DashboardComponent,
    canActivate: [AuthGuard],
  },
];
