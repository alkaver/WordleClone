import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserDetail } from 'app/interfaces/user-detail';
import { AuthService } from 'app/services/auth.service';
import { RoleService, RoleResponseDto, RoleAssignDto } from 'app/services/role.service';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
  users = signal<UserDetail[]>([]);
  roles = signal<RoleResponseDto[]>([]);
  loadingUsers = signal(false);
  loadingRoles = signal(false);
  errorUsers = signal('');
  errorRoles = signal('');

  newRoleName = '';
  selectedUserId = '';
  selectedRoleName = '';

  constructor(private authService: AuthService, private roleService: RoleService) {}

  ngOnInit(): void {
    this.fetchUsers();
    this.fetchRoles();
  }

  fetchUsers(): void {
    this.loadingUsers.set(true);
    this.authService.getAllUsers().subscribe({
      next: (data) => {
        this.users.set(data);
        this.loadingUsers.set(false);
      },
      error: () => {
        this.errorUsers.set('Błąd pobierania użytkowników.');
        this.loadingUsers.set(false);
      }
    });
  }

  fetchRoles(): void {
    this.loadingRoles.set(true);
    this.roleService.getRoles().subscribe({
      next: (data) => {
        this.roles.set(data);
        this.loadingRoles.set(false);
      },
      error: () => {
        this.errorRoles.set('Błąd pobierania ról.');
        this.loadingRoles.set(false);
      }
    });
  }

  createRole(): void {
    if (!this.newRoleName.trim()) return;
    this.roleService.createRole(this.newRoleName).subscribe(() => {
      this.newRoleName = '';
      this.fetchRoles();
    });
  }

  assignRole(): void {
    if (!this.selectedUserId || !this.selectedRoleName) return;
    const dto: RoleAssignDto = {
      userId: this.selectedUserId,
      roleName: this.selectedRoleName
    };
    this.roleService.assignRole(dto).subscribe(() => {
      this.fetchUsers(); // aktualizacja użytkowników
    });
  }

  deleteRole(roleName: string): void {
    this.roleService.deleteRole(roleName).subscribe(() => {
      this.fetchRoles();
    });
  }
}
