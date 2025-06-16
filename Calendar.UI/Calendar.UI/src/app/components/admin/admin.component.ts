import { Component, OnInit } from '@angular/core';
import { UserDetail } from 'app/interfaces/user-detail';
import { AuthService } from 'app/services/auth.service';

@Component({
  selector: 'app-admin',
  imports: [],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  users: UserDetail[] = [];
  loading = false;
  error = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.loading = true;
    this.authService.getAllUsers().subscribe({
      next: users => {
        this.users = users;
        this.loading = false;
      },
      error: () => {
        this.error = 'Wystąpił błąd podczas pobierania użytkowników.';
        this.loading = false;
      }
    });
  }
}
