import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ResultDto } from '../models/result.dto';
import { RestService } from './rest.service';
import { RoleApiUrlConstant } from '../constants/role-api-url.constant';
import { RoleDto } from '../models/role.dto';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  constructor(private restService: RestService) {}

  getAll(): Observable<ResultDto<string[]>> {
    return this.restService.get<ResultDto<string[]>>(`${RoleApiUrlConstant.getAll}`);
  }

  create(body: RoleDto): Observable<ResultDto<string>> {
    return this.restService.post<ResultDto<string>>(`${RoleApiUrlConstant.getAll}`, body);
  }
}
