import { Injectable } from '@angular/core';
import { LoginDto } from '../models/login.dto';
import { BehaviorSubject, catchError, Observable, of, tap } from 'rxjs';
import { RestService } from './rest.service';
import { ResultDto } from '../models/result.dto';
import { RegisterDto } from '../models/register.dto';
import { UserApiUrlConstant } from '../constants/user-api-url.constant';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private loggedInSubject = new BehaviorSubject<boolean>(false);
  loggedIn$ = this.loggedInSubject.asObservable();

  constructor(private restService: RestService) {}

  login(request: LoginDto): Observable<ResultDto<string>> {
    return this.restService
      .post<ResultDto<string>>(`${UserApiUrlConstant.login}`, request)
      .pipe(
        tap((result: ResultDto<string>) => this.loggedInSubject.next(result.result ? true : false)),
      );
  }

  register(request: RegisterDto): Observable<ResultDto<string>> {
    return this.restService
      .post<ResultDto<string>>(`${UserApiUrlConstant.register}`, request)
      .pipe(
        tap((result: ResultDto<string>) => this.loggedInSubject.next(result.result ? true : false)),
      );
  }

  logout(): Observable<ResultDto<boolean>> {
    return this.restService
      .post<ResultDto<boolean>>(`${UserApiUrlConstant.logout}`, null)
      .pipe(
        tap((result: ResultDto<boolean>) =>
          this.loggedInSubject.next(result.result ? false : true),
        ),
      );
  }

  checkAuth(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${UserApiUrlConstant.checkAuth}`).pipe(
      tap((result: ResultDto<boolean>) => {
        this.loggedInSubject.next(result.result ?? false);
      }),
    );
  }

  amIAdmin(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${UserApiUrlConstant.amIAdmin}`);
  }

  setLoggedIn(value: boolean): void {
    this.loggedInSubject.next(value);
  }
}
