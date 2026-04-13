import { Component, inject } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  readonly auth = inject(AuthService);
  readonly isLoggedIn = this.auth.isLoggedIn();

  menuItems = [
    { label: 'Dashboard',    icon: '📊', route: '/dashboard',  adminOnly: false },
    { label: 'Candidates',   icon: '👤', route: '/candidates', adminOnly: false },
    { label: 'Job Postings', icon: '💼', route: '/jobs',       adminOnly: false },
    { label: 'Pipeline',     icon: '🗂', route: '/pipeline',   adminOnly: false },
    { label: 'Analytics',    icon: '📈', route: '/analytics',  adminOnly: false },
    { label: 'Users',        icon: '🔑', route: '/users',      adminOnly: true  },
  ];
}
