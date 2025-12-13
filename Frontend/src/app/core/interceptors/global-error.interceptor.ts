import {
  HttpEvent,
  HttpRequest,
  HttpInterceptorFn,
  HttpHandlerFn,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpStatus } from '../constants/http-status.constant';
import { inject } from '@angular/core';
import { UserService } from '../services/user.service';
import { Router } from '@angular/router';
import { RouteConstants } from '../constants/route.constant';

export const globalErrorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const userService = inject(UserService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case HttpStatus.UNAUTHORIZED:
          userService.setLoggedIn(false);
          router.navigate([RouteConstants.LOGIN_PATH]);
          break;
        default:
          break;
      }

      return throwError(() => error);
    }),
  );
};
