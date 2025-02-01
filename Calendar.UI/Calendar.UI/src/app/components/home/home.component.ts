import { CommonModule, NgForOf} from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { WordsService } from '../../services/words.service';
import { GamesService } from '../../services/games.service';

@Component({
  selector: 'app-home',
  imports: [NgForOf, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  solution: string = '';
  gameWon: boolean = false;
  winMessage: string = '';
  loseMessage: string ='';

  
  rows = Array(6).fill(null);
  cols = Array(5).fill(null);
  row1 = ['Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'];
  row2 = ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L'];
  row3 = ['Z', 'X', 'C', 'V', 'B', 'N', 'M'];

  grid: string[][] = this.rows.map(() => Array(5).fill(''));
  validationGrid: string[][] = this.rows.map(() => Array(5).fill(''));
  currentRow = 0;
  
  // Stan liter na klawiaturze
  keyboardState: { [key: string]: 'correct' | 'present' | 'absent' } = {};

  constructor(private wordsService: WordsService, private gamesService: GamesService) {}

  ngOnInit(): void {
    this.checkForMidnightReset();
    this.loadGameState();
    console.log(this.solution);
    if (this.solution === '') {
      this.wordsService.getWord().subscribe((response: string) => {
        this.solution = response.trim().toUpperCase();
        this.saveGameState();
      });
    }
  }

  onKeyClick(key: string) {
    if (this.gameWon) {
      return; // Zatrzymanie dalszego wpisywania po wygranej
    }

    if (key === 'ENTER') {
      this.validateRow();
    } else if (key === 'BACKSPACE') {
      this.removeLetter();
    } else {
      this.fillLetter(key);
    }
  }

  // Wypełnianie liter
  fillLetter(letter: string) {
    const emptyIndex = this.grid[this.currentRow].indexOf('');
    if (emptyIndex !== -1) {
      this.grid[this.currentRow][emptyIndex] = letter.toUpperCase();
    }
  }

  // Walidacja wiersza
  validateRow() {
    const word = this.grid[this.currentRow].join('');
    if (word.length < 5) return;
  
    const solutionArray = this.solution.split('');
    const guessedArray = this.grid[this.currentRow].slice();
  
    // Reset walidacji dla wiersza
    this.validationGrid[this.currentRow] = Array(5).fill('');
  
    // Walidacja poprawnych miejsc (correct)
    guessedArray.forEach((letter, index) => {
      if (letter === solutionArray[index]) {
        this.validationGrid[this.currentRow][index] = 'correct';
        this.keyboardState[letter] = 'correct';  // Aktualizacja klawiatury
        solutionArray[index] = '';
        guessedArray[index] = '';
      }
    });
  
    // Walidacja liter na złych pozycjach (present)
    guessedArray.forEach((letter, index) => {
      if (letter && solutionArray.includes(letter)) {
        this.validationGrid[this.currentRow][index] = 'present';
        if (!this.keyboardState[letter]) {
          this.keyboardState[letter] = 'present';  // Tylko jeśli nie jest już correct
        }
        solutionArray[solutionArray.indexOf(letter)] = '';
      } else if (letter) {
        this.validationGrid[this.currentRow][index] = 'absent';
        if (!this.keyboardState[letter]) {
          this.keyboardState[letter] = 'absent';
        }
      }
    });

    // Sprawdzenie, czy gracz wygrał
    if (this.validationGrid[this.currentRow].every(cell => cell === 'correct')) {
      this.gameWon = true;
      this.winMessage = '🎉 Fantastic job! You guessed the word correctly! 🎉\nCome back tomorrow for a new challenge!';
      this.saveGameState();
    
      // Wywołanie API
      this.gamesService.playGameResult(this.gameWon).subscribe({
        next: (response) => {
          console.log('Game result sent successfully:', response);
        },
        error: (err) => {
          console.error('Error sending game result:', err);
        }
      });
    }

    // Sprawdzenie, czy gracz przegrał
    if (this.currentRow === 5 && !this.gameWon) {
      this.gameWon = false;
      this.loseMessage = `You lost! The correct word was: ${this.solution}. Come back tomorrow to try again!`;
      this.saveGameState();

      // Wywołanie API
      this.gamesService.playGameResult(this.gameWon).subscribe({
          next: (response) => {
              console.log('Game result sent successfully:', response);
          },
          error: (err) => {
              console.error('Error sending game result:', err);
          }
      });

      return; // Nie przechodź do następnego wiersza
  }
    

    // Przejście do następnego wiersza, ale tylko jeśli nie ma wygranej
    if (!this.gameWon) {
      this.currentRow++;
      this.saveGameState();
    }
  }
  
  // Funkcja, która zwraca odpowiednią klasę dla komórki
  getCellClass(row: number, col: number): string {
    const letter = this.validationGrid[row][col];
    if (letter === 'correct') {
      return 'correct';
    } else if (letter === 'present') {
      return 'present';
    } else if (letter === 'absent') {
      return 'absent';
    } else {
      return 'empty';
    }
  }

  // Obsługuje klawiaturę
  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    const key = event.key.toUpperCase();

    if (this.gameWon) {
      return; // Zatrzymanie dalszego wpisywania po wygranej
    }

    // Jeśli naciśnięty klawisz to litera (A-Z)
    if (/^[A-Z]$/.test(key)) {
      this.fillLetter(key);
    }

    // Obsługuje Enter
    if (key === 'ENTER') {
      this.validateRow();
    }

    // Obsługuje Backspace
    if (key === 'BACKSPACE') {
      this.removeLetter();
    }
  }

  // Usuwanie ostatniej litery w bieżącym wierszu
  removeLetter() {
    let lastNonEmptyIndex = this.grid[this.currentRow].length - 1;
    while (lastNonEmptyIndex >= 0 && this.grid[this.currentRow][lastNonEmptyIndex] === '') {
      lastNonEmptyIndex--;
    }
    if (lastNonEmptyIndex >= 0) {
      this.grid[this.currentRow][lastNonEmptyIndex] = '';
    }
  }

  // Zapisywanie stanu gry

  saveGameState() {
    const gameState = {
      grid: this.grid,
      validationGrid: this.validationGrid,
      currentRow: this.currentRow,
      gameWon: this.gameWon,
      loseMessage: this.loseMessage,
      winMessage: this.winMessage,
      keyboardState: this.keyboardState,
      timestamp: new Date().getTime()
    };
    localStorage.setItem('wordleGameState', JSON.stringify(gameState));
  }

  loadGameState() {
    const savedState = localStorage.getItem('wordleGameState');
    if (savedState) {
      const gameState = JSON.parse(savedState);
      this.grid = gameState.grid;
      this.validationGrid = gameState.validationGrid;
      this.currentRow = gameState.currentRow;
      this.gameWon = gameState.gameWon;
      this.loseMessage = gameState.loseMessage,
      this.winMessage = gameState.winMessage;
      this.keyboardState = gameState.keyboardState;
    }
  }
  checkForMidnightReset() {
    const savedState = localStorage.getItem('wordleGameState');
    if (savedState) {
      const gameState = JSON.parse(savedState);
      const now = new Date();
      const savedTime = new Date(gameState.timestamp);
      console.log('Saved Timestamp:', gameState.timestamp);
      console.log('Current Time:', new Date().getTime());

      if (now.toDateString() !== savedTime.toDateString()) {
        localStorage.removeItem('wordleGameState');
        this.grid = this.rows.map(() => Array(5).fill(''));
        this.validationGrid = this.rows.map(() => Array(5).fill(''));
        this.currentRow = 0;
        this.gameWon = false;
        this.loseMessage = '',
        this.winMessage = '';
        this.keyboardState = {};
      }
    }
  }
}
