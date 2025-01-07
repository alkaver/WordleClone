import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-auth',
  standalone: true,  // Ważne dla komponentów standalone
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
  imports: [ReactiveFormsModule, CommonModule]  // Importowanie ReactiveFormsModule lokalnie
})
export class AuthComponent {
  authForm: FormGroup;
  isLoginMode = true;
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.authForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(9),
          Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*])/)
        ]
      ]
    });
  }

  onSwitchMode() {
    this.isLoginMode = !this.isLoginMode;
  }

  onSubmit() {
    if (this.authForm.invalid) {
      this.errorMessage = 'Formularz zawiera błędy.';
      return;
    }

    if (this.isLoginMode) {
      this.authService.login(this.authForm.value).subscribe({
        next: () => {
          console.log('Zalogowano!');
        },
        error: () => {
          this.errorMessage = 'Nieprawidłowe dane logowania';
        }
        });
    }
    else {
      this.authService.register(this.authForm.value).subscribe({
        next: () => {
          this.isLoginMode = true;
          this.errorMessage = 'Rejestracja udana. Możesz się zalogować.';
        },
        error: () => {
          this.errorMessage = 'Rejestracja nie powiodła się.';
        }
      }); 
    }
  }
}
