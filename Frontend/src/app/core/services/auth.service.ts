import { Injectable } from '@angular/core';
import { LoginDto } from '../models/login.dto';
import { Observable } from 'rxjs';
import { RestService } from './rest.service';
import { ResultDto } from '../models/result.dto';
import { RegisterDto } from '../models/register.dto';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly baseUrl = 'https://localhost:8081/api/User';

  constructor(private restService: RestService) {}

  login(request: LoginDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${this.baseUrl}/login`, request);
  }

  register(request: RegisterDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${this.baseUrl}/register`, request);
  }

  logout(): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${this.baseUrl}/logout`, null);
  }

  checkAuth(): Observable<ResultDto<boolean>> {
    return this.restService.get<ResultDto<boolean>>(`${this.baseUrl}/checkAuth`);
  }
}
