import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class RestService {
  constructor(protected http: HttpClient) {}

  public post<T>(url: string, body: any, options?: { headers?: HttpHeaders }): Observable<T> {
    return this.http.post<T>(url, body, {
      ...options,
      withCredentials: true,
    });
  }

  public get<T>(url: string, options?: { headers?: HttpHeaders }): Observable<T> {
    return this.http.get<T>(url, {
      ...options,
      withCredentials: true,
    });
  }
}
