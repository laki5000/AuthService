import { Inject, Injectable, PLATFORM_ID } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { AuthService } from "../services/auth.service";
import { catchError, map, Observable, of } from "rxjs";
import { ResultDto } from "../models/result.dto";
import { isPlatformServer } from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService, 
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: any
  ) {}

  private LOGIN_PATH: string = '/login';

  canActivate(): Observable<boolean> {
    if (isPlatformServer(this.platformId)) {
      return of(true);
    }
    
    return this.authService.checkAuth().pipe(
      map((result: ResultDto<string>) => {
        if (result.success) {
          return true; 
        } else {
          this.router.navigate([this.LOGIN_PATH]);
          return false;
        }
      }),
      catchError((error: any) => {
        this.router.navigate([this.LOGIN_PATH]);
        return of(false);
      })
    );
  }
}
