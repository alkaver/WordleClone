import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class WordsService {
  private url = "Word/wordoftheday";
  constructor(private http: HttpClient) { }
  public getWord(): Observable<string> {
    return this.http.get(`${environment.apiUrl}/${this.url}`, { responseType: 'text' });
  }
}
