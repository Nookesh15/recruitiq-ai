import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface StageFunnelItem {
  stage: string;
  count: number;
  dropOffPct: number;
}

export interface FunnelResult {
  funnel: StageFunnelItem[];
  total: number;
  avgScore: number | null;
  avgDaysToHire: number | null;
}

@Injectable({ providedIn: 'root' })
export class AnalyticsService {
  private readonly http = inject(HttpClient);

  getFunnel(jobId?: string): Observable<FunnelResult> {
    const params = jobId ? `?jobId=${jobId}` : '';
    return this.http.get<FunnelResult>(`${environment.apiUrl}/analytics/funnel${params}`);
  }
}
