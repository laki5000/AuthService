import { HttpEvent, HttpRequest, HttpInterceptorFn, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpStatus } from '../constants/http-status.constant';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { RouteConstants } from '../constants/route.constant';

export const globalErrorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case HttpStatus.Unauthorized:
          authService.setLoggedIn(false);
          router.navigate([RouteConstants.LoginPath]);
          break;
        default:
          break;
      }

      return throwError(() => error);
    })
  );
};
