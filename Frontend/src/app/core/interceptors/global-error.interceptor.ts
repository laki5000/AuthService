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
import { AuthService } from '../services/auth.service';

export const globalErrorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case HttpStatus.UNAUTHORIZED:
          authService.setLoggedIn(false);
          break;
        default:
          break;
      }

      return throwError(() => error);
    }),
  );
};
