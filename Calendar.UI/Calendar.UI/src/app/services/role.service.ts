import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';

export interface RoleResponseDto {
  id: string;
  name: string;
  totalUsers: number;
}

export interface RoleAssignDto {
  userId: string;
  roleName: string;
}

@Injectable({ providedIn: 'root' })
export class RoleService {
  private baseUrl = `${environment.apiUrl}/RolesContoller`; 

  constructor(private http: HttpClient) {}

  getRoles(): Observable<RoleResponseDto[]> {
    return this.http.get<RoleResponseDto[]>(`${this.baseUrl}/getRoles`);
  }

  createRole(roleName: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/createRole`, { roleName });
  }

  assignRole(dto: RoleAssignDto): Observable<any> {
    return this.http.post(`${this.baseUrl}/assignRole`, dto);
  }

  deleteRole(roleName: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/deleteRole?roleName=${encodeURIComponent(roleName)}`);
  }
}
