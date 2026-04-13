import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent),
  },
  {
    // Public apply portal — no auth required
    path: 'apply/:jobId',
    loadComponent: () => import('./features/portal/apply.component').then(m => m.ApplyComponent),
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'candidates',
        loadComponent: () => import('./features/candidates/candidates.component').then(m => m.CandidatesComponent),
      },
      {
        path: 'candidates/:id',
        loadComponent: () => import('./features/candidates/candidate-detail.component').then(m => m.CandidateDetailComponent),
      },
      {
        path: 'jobs',
        loadComponent: () => import('./features/jobs/jobs.component').then(m => m.JobsComponent),
      },
      {
        path: 'jobs/:id/applicants',
        loadComponent: () => import('./features/jobs/job-applicants.component').then(m => m.JobApplicantsComponent),
      },
    ],
  },
  { path: '**', redirectTo: 'dashboard' },
];
