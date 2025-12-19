import { Injectable } from '@angular/core';
import { LoginDto } from '../models/login.dto';
import { BehaviorSubject, catchError, Observable, of, skip, Subject, Subscription, takeUntil, tap } from 'rxjs';
import { RestService } from './rest.service';
import { ResultDto } from '../models/result.dto';
import { RegisterDto } from '../models/register.dto';
import { AuthApiUrlConstant } from '../constants/auth-api-url.constant';
import { Router } from '@angular/router';
import { RouteConstants } from '../constants/route.constant';
import { UpdateUserRoleDto } from '../models/update-user-role.dto';
import { RoleDto } from '../models/role.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private destroy$ = new Subject<void>();
  
  private loggedInSubject = new BehaviorSubject<boolean>(false);
  loggedIn$ = this.loggedInSubject.asObservable();
  private loggedInSub?: Subscription;

  constructor(
    private restService: RestService,
    private router: Router,
  ) {}

  login(body: LoginDto): Observable<ResultDto<string>> {
    return this.restService
      .post<ResultDto<string>>(`${AuthApiUrlConstant.login}`, body)
      .pipe(
        tap((result: ResultDto<string>) => this.loggedInSubject.next(result.Result ? true : false)),
      );
  }

  register(body: RegisterDto): Observable<ResultDto<string>> {
    return this.restService
      .post<ResultDto<string>>(`${AuthApiUrlConstant.register}`, body)
      .pipe(
        tap((result: ResultDto<string>) => this.loggedInSubject.next(result.Result ? true : false)),
      );
  }

  logout(): Observable<ResultDto<boolean>> {
    return this.restService
      .post<ResultDto<boolean>>(`${AuthApiUrlConstant.logout}`, null)
      .pipe(
        tap((result: ResultDto<boolean>) =>
          this.loggedInSubject.next(result.Result ? false : true),
        ),
      );
  }

  checkAuth(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${AuthApiUrlConstant.checkAuth}`).pipe(
      tap((result: ResultDto<boolean>) => {
        this.loggedInSubject.next(result.Result ?? false);
      }),
    );
  }

  amIAdmin(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${AuthApiUrlConstant.amIAdmin}`);
  }

  updateUserRole(body: UpdateUserRoleDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${AuthApiUrlConstant.updateUserRole}`, body);
  }

  getAllRoles(): Observable<ResultDto<string[]>> {
    return this.restService.get<ResultDto<string[]>>(`${AuthApiUrlConstant.getAllRoles}`);
  }

  createRole(body: RoleDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${AuthApiUrlConstant.createRole}`, body);
  }

  setLoggedIn(value: boolean): void {
    this.loggedInSubject.next(value);
  }

  monitorLoginStatus(): void {
    this.loggedIn$.pipe(skip(1), takeUntil(this.destroy$)).subscribe((loggedIn: boolean) => {
      if (!loggedIn) {
        this.router.navigate([RouteConstants.LOGIN_PATH]);
      }
    });
  }

  stopMonitorLoginStatus(): void {
    this.destroy$.next(); 
    this.destroy$.complete();
  }
}
