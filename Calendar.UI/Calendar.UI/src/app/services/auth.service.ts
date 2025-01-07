import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment.development';
import { LoginRequest } from '../interfaces/login-request';
import { AuthResponse } from '../interfaces/auth-response';
import { jwtDecode } from 'jwt-decode';
import { UserDetail } from '../interfaces/user-detail';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private Url = 'Account';  // Główny URL API
  private tokenKey = 'token';

  constructor(private http: HttpClient, private router: Router) {}

  login(data:LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/${this.Url}/login`, data).pipe(
      map((response)=>
      {
        if(response.isSuccess)
        {
          localStorage.setItem(this.tokenKey,response.token);
        // Odczytaj returnUrl z localStorage
        const returnUrl = localStorage.getItem('returnUrl') || '/ranking';
        this.router.navigate([returnUrl]);
        localStorage.removeItem('returnUrl');  
        }
        return response;
      })
    );
  }

  public getToken = ():string | null => localStorage.getItem(this.tokenKey) || '';
  
  public getDetail = ():Observable<UserDetail> =>
    this.http.get<UserDetail>(`${environment.apiUrl}/${this.Url}/detail`)

  isLoggedIN = ():boolean =>{
    const token = this.getToken();
    if(!token) return false;
    return !this.isTokenExpired();
  }

  private isTokenExpired()
  {
    const token = this.getToken();
    if(!token) return true;
    const decoded = jwtDecode(token);
    const isTokenExpired = Date.now() >= decoded['exp']! * 1000;
    if(isTokenExpired) this.logout();
    return isTokenExpired
  }

  register(data:LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/${this.Url}/register`, data);
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
  }
}


