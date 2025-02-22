import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class GamesService {
  private url = "Game/play";
  constructor(private http: HttpClient) {}

  playGameResult(WonGame: boolean): Observable<any> {
    return this.http.post(`${environment.apiUrl}/${this.url}`, { WonGame });
  }
  
}
