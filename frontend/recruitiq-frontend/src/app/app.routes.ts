import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
  },
  {
    path: 'candidates',
    loadComponent: () => import('./features/candidates/candidates.component').then(m => m.CandidatesComponent),
  },
  { path: '**', redirectTo: 'dashboard' },
];
