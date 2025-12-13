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

export const globalErrorInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> => {
  const userService = inject(UserService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      switch (error.status) {
        case HttpStatus.UNAUTHORIZED:
          userService.setLoggedIn(false);
          break;
        default:
          break;
      }

      return throwError(() => error);
    }),
  );
};
