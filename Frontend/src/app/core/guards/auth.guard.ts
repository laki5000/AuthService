import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { catchError, map, Observable, of } from 'rxjs';
import { ResultDto } from '../models/result.dto';
import { isPlatformServer } from '@angular/common';
import { RouteConstants } from '../constants/route.constant';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: any,
  ) {}

  canActivate(): Observable<boolean> {
    if (isPlatformServer(this.platformId)) {
      return of(true);
    }

    return this.authService.checkAuth().pipe(
      map((result: ResultDto<boolean>) => {
        if (result.result) return true;

        this.router.navigate([RouteConstants.LoginPath]);
        return false;
      })
    );
  }
}
