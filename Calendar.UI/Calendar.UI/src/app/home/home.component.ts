import { CommonModule, NgForOf} from '@angular/common';
import { Component, HostListener } from '@angular/core';
import { WordsService } from '../services/words.service';

@Component({
  selector: 'app-home',
  imports: [NgForOf, CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {
  solution: string = '';

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

  // Obsługa klawiatury
  onKeyClick(key: string) {
    if (key === 'ENTER') {
      this.validateRow();
    } else if (key === 'BACKSPACE') {
      this.removeLetter();
    } else {
      this.fillLetter(key);
    }
  }


  grid: string[][] = this.rows.map(() => Array(5).fill('')); // Tablica przechowująca litery
  validationGrid: string[][] = this.rows.map(() => Array(5).fill('')); // Tablica do walidacji (klasy do kolorowania)
  currentRow = 0;

  // Wypełnianie liter - tylko przykład
  fillLetter(letter: string) {
    const emptyIndex = this.grid[this.currentRow].indexOf('');
    if (emptyIndex !== -1) {
      this.grid[this.currentRow][emptyIndex] = letter.toUpperCase();
    }
  }

  // Walidacja wiersza (w tym przypadku, tylko przypisanie klas do validationGrid)
  validateRow() {
    const word = this.grid[this.currentRow].join('');
    if (word.length < 5) return;

    const solutionArray = this.solution.split('');
    const guessedArray = this.grid[this.currentRow].slice();

    // Resetowanie tablicy validationGrid dla bieżącego wiersza
    this.validationGrid[this.currentRow] = Array(5).fill('');

    // Walidacja - poprawne miejsce
    guessedArray.forEach((letter, index) => {
      if (letter === solutionArray[index]) {
        this.validationGrid[this.currentRow][index] = 'correct';
        solutionArray[index] = '';
        guessedArray[index] = '';
      }
    });

    // Walidacja - litery w innym miejscu
    guessedArray.forEach((letter, index) => {
      if (letter && solutionArray.includes(letter)) {
        this.validationGrid[this.currentRow][index] = 'present';
        solutionArray[solutionArray.indexOf(letter)] = '';
      } else if (letter) {
        this.validationGrid[this.currentRow][index] = 'absent';
      }
    });

    // Przejście do następnego wiersza
    this.currentRow++;
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
    // Szukamy ostatniej niepustej komórki w bieżącym wierszu
    let lastNonEmptyIndex = this.grid[this.currentRow].length - 1;
    
    // Znajdujemy ostatnią niepustą literę (komórkę)
    while (lastNonEmptyIndex >= 0 && this.grid[this.currentRow][lastNonEmptyIndex] === '') {
      lastNonEmptyIndex--;
    }
    
    // Jeśli znaleziono niepustą komórkę, to usuwamy literę z tej komórki
    if (lastNonEmptyIndex >= 0) {
      this.grid[this.currentRow][lastNonEmptyIndex] = '';
    }
  }
}
