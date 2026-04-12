import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  menuItems = [
    { label: 'Dashboard', icon: '📊', route: '/dashboard' },
    { label: 'Candidates', icon: '👤', route: '/candidates' },
    { label: 'Job Postings', icon: '💼', route: '/jobs' },
    { label: 'AI Screening', icon: '🤖', route: '/screening' },
    { label: 'Analytics', icon: '📈', route: '/analytics' },
  ];
}
