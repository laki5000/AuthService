import {
  HttpEvent,
  HttpRequest,
  HttpHandlerFn,
  HttpInterceptorFn,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ResultDto } from '../models/result.dto';
import { ApiMessageConstants } from '../constants/api-message.constant';

export const globalResponseInterceptor: HttpInterceptorFn = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {

  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    tap({
      next: (event) => {
        if (event instanceof HttpResponse) {
          snackBar.open(ApiMessageConstants.SUCCESS, ApiMessageConstants.DONE, { duration: 2000 });
        }
      },
      error: (error: HttpErrorResponse) => {
        const errorBody = error.error as ResultDto<string>;
        const message = errorBody?.Error || ApiMessageConstants.DEFAULT_ERROR;
        snackBar.open(`${ApiMessageConstants.FAILED}: ${message}`, ApiMessageConstants.DONE, { duration: 3000 });
      }
    })
  );
};