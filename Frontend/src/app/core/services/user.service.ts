import { Injectable } from '@angular/core';
import { LoginDto } from '../models/login.dto';
import { BehaviorSubject, catchError, Observable, of, skip, tap } from 'rxjs';
import { RestService } from './rest.service';
import { ResultDto } from '../models/result.dto';
import { RegisterDto } from '../models/register.dto';
import { UserApiUrlConstant } from '../constants/user-api-url.constant';
import { Router } from '@angular/router';
import { RouteConstants } from '../constants/route.constant';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private loggedInSubject = new BehaviorSubject<boolean>(false);
  loggedIn$ = this.loggedInSubject.asObservable();

  constructor(
    private restService: RestService,
    private router: Router,
  ) {}

  login(request: LoginDto): Observable<ResultDto<string>> {
    return this.restService
      .post<ResultDto<string>>(`${UserApiUrlConstant.login}`, request)
      .pipe(
        tap((result: ResultDto<string>) => this.loggedInSubject.next(result.Result ? true : false)),
      );
  }

  register(request: RegisterDto): Observable<ResultDto<string>> {
    return this.restService
      .post<ResultDto<string>>(`${UserApiUrlConstant.register}`, request)
      .pipe(
        tap((result: ResultDto<string>) => this.loggedInSubject.next(result.Result ? true : false)),
      );
  }

  logout(): Observable<ResultDto<boolean>> {
    return this.restService
      .post<ResultDto<boolean>>(`${UserApiUrlConstant.logout}`, null)
      .pipe(
        tap((result: ResultDto<boolean>) =>
          this.loggedInSubject.next(result.Result ? false : true),
        ),
      );
  }

  checkAuth(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${UserApiUrlConstant.checkAuth}`).pipe(
      tap((result: ResultDto<boolean>) => {
        this.loggedInSubject.next(result.Result ?? false);
      }),
    );
  }

  amIAdmin(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${UserApiUrlConstant.amIAdmin}`);
  }

  setLoggedIn(value: boolean): void {
    this.loggedInSubject.next(value);
  }

  monitorLoginStatus(): void {
    this.loggedIn$.pipe(skip(1)).subscribe((loggedIn: boolean) => {
      if (loggedIn === false) {
        this.router.navigate([RouteConstants.LOGIN_PATH]);
      }
    });
  }
}
