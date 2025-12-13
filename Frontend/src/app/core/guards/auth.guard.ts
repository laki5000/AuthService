import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { UserService } from '../services/user.service';
import { catchError, map, Observable, of } from 'rxjs';
import { ResultDto } from '../models/result.dto';
import { isPlatformServer } from '@angular/common';
import { RouteConstants } from '../constants/route.constant';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private userService: UserService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: any,
  ) {}

  canActivate(): Observable<boolean> {
    if (isPlatformServer(this.platformId)) {
      return of(true);
    }

    return this.userService.checkAuth().pipe(
      map((result: ResultDto<boolean>) => {
        if (result.Result) return true;

        this.router.navigate([RouteConstants.LOGIN_PATH]);
        return false;
      }),
    );
  }
}
