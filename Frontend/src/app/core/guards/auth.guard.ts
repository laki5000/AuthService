import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { AuthService } from "../services/auth.service";
import { catchError, map, Observable, of } from "rxjs";
import { ResultDto } from "../models/result.dto";

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): Observable<boolean> {
    console.log('AuthGuard: Checking authentication status');
    return this.authService.checkAuth().pipe(
      map((result: ResultDto<string>) => {
        if (result.success) {
          return true; 
        } else {
          this.router.navigate(['/login']);
          return false;
        }
      }),
      catchError((error: any) => {
        console.log(error);
        //this.router.navigate(['/login']);
        return of(false);
      })
    );
  }
}
