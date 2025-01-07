import { CommonModule, NgForOf} from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { WordsService } from '../../services/words.service';

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
  
  // Stan liter na klawiaturze
  keyboardState: { [key: string]: 'correct' | 'present' | 'absent' } = {};

  constructor(private wordsService: WordsService) {}

  ngOnInit(): void {
    this.wordsService.getWord().subscribe(
      (response: string) => {
        this.solution = response.trim().toUpperCase();
        console.log('Wylosowane słowo:', this.solution);
      }
    )
  }

  rows = Array(6).fill(null);
  cols = Array(5).fill(null);
  row1 = ['Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'];
  row2 = ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L'];
  row3 = ['Z', 'X', 'C', 'V', 'B', 'N', 'M'];

  grid: string[][] = this.rows.map(() => Array(5).fill(''));
  validationGrid: string[][] = this.rows.map(() => Array(5).fill(''));
  currentRow = 0;

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
      this.winMessage = 'Gratulacje! Zgadłeś słowo!';
    }

    // Przejście do następnego wiersza, ale tylko jeśli nie ma wygranej
    if (!this.gameWon) {
      this.currentRow++;
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
}
