import { Injectable } from '@angular/core';
import { LoginDto } from '../models/login.dto';
import { BehaviorSubject, catchError, Observable, of, tap } from 'rxjs';
import { RestService } from './rest.service';
import { ResultDto } from '../models/result.dto';
import { RegisterDto } from '../models/register.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly baseUrl = 'https://localhost:8081/api/User';
  
  private loggedInSubject = new BehaviorSubject<boolean>(false);
  loggedIn$ = this.loggedInSubject.asObservable();

  constructor(private restService: RestService) {}

  login(request: LoginDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${this.baseUrl}/login`, request).pipe(
      tap((result: ResultDto<string>) => this.loggedInSubject.next(result.result ? true : false))
    );
  }

  register(request: RegisterDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${this.baseUrl}/register`, request).pipe(
      tap((result: ResultDto<string>) => this.loggedInSubject.next(result.result ? true : false))
    );
  }

  logout(): Observable<ResultDto<boolean>> {
    return this.restService.post<ResultDto<boolean>>(`${this.baseUrl}/logout`, null).pipe(
      tap((result: ResultDto<boolean>) => this.loggedInSubject.next(result.result ? false : true))
    );
  }

  checkAuth(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${this.baseUrl}/checkAuth`).pipe(
      tap((result: ResultDto<boolean>) => {
        this.loggedInSubject.next(result.result ?? false);
      })
    );
  }

  public setLoggedIn(value: boolean): void {
    this.loggedInSubject.next(value);
  }
}
