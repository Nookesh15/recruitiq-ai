import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface UserRow {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [NgClass, FormsModule],
  templateUrl: './users.component.html',
})
export class UsersComponent implements OnInit {
  private readonly http = inject(HttpClient);

  users: UserRow[] = [];
  loading = true;
  error = '';

  // Create form
  showCreate = false;
  creating = false;
  createError = '';
  form = { email: '', password: '', firstName: '', lastName: '', role: 'Recruiter' };

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.http.get<UserRow[]>(`${environment.apiUrl}/auth/users`).subscribe({
      next: (u) => { this.users = u; this.loading = false; },
      error: () => { this.error = 'Failed to load users'; this.loading = false; },
    });
  }

  create(): void {
    this.creating = true;
    this.createError = '';
    this.http.post(`${environment.apiUrl}/auth/register`, this.form).subscribe({
      next: (u: any) => {
        this.users = [u, ...this.users];
        this.showCreate = false;
        this.form = { email: '', password: '', firstName: '', lastName: '', role: 'Recruiter' };
        this.creating = false;
      },
      error: (err) => {
        this.createError = err?.error?.error ?? 'Failed to create user.';
        this.creating = false;
      },
    });
  }

  roleColor(role: string): string {
    return role === 'Admin'
      ? 'bg-purple-100 text-purple-700'
      : 'bg-blue-100 text-blue-700';
  }
}
