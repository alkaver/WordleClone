import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-header',
  imports: [RouterLink, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  constructor(public authService: AuthService) {}
  title = signal('Wordle Clone')
    ngOnInit(): void {
    this.authService.updateUserRoles();  
  }
}
