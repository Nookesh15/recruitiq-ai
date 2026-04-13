import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface DashboardStats {
  totalCandidates: number;
  activeJobs: number;
  totalApplications: number;
  avgAiScore: number | null;
  scoredCount: number;
  pipeline: { status: string; count: number }[];
  recentApplications: {
    id: string;
    candidateName: string;
    email: string;
    jobTitle: string;
    matchScore: number | null;
    createdAt: string;
  }[];
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${environment.apiUrl}/dashboard/stats`);
  }
}
