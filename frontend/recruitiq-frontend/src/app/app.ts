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
    { label: 'Dashboard', icon: '📊', route: '/dashboard' },
    { label: 'Candidates', icon: '👤', route: '/candidates' },
    { label: 'Job Postings', icon: '💼', route: '/jobs' },
    { label: 'Pipeline', icon: '🗂', route: '/pipeline' },
    { label: 'Analytics', icon: '📈', route: '/analytics' },
  ];
}
