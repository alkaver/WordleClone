import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  private apiUrl = 'https://localhost:7059/api/Game';  // Ustaw swój URL do API

  constructor(private http: HttpClient) {}

  startGame(word: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/start?word=${word}`, {});
  }

  makeGuess(userId: number, gameId: number, guess: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/guess`, { userId, gameId, guess });
  }

  getUserGuesses(gameId: number, userId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/game/${gameId}/user/${userId}`);
  }
}

