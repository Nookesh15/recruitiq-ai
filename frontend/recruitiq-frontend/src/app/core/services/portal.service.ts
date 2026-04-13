import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface PublicJob {
  id: string;
  title: string;
  description: string;
  department: string;
  location: string;
  status: string;
  createdAt: string;
}

export interface ApplyResult {
  applicationId: string;
  candidateId: string;
  candidateName: string;
  email: string;
  jobPostingId: string;
  jobTitle: string;
  aiScore: number | null;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class PortalService {
  private readonly http = inject(HttpClient);

  getJob(jobId: string): Observable<PublicJob> {
    return this.http.get<PublicJob>(`${environment.apiUrl}/portal/jobs/${jobId}`);
  }

  apply(jobId: string, form: FormData): Observable<ApplyResult> {
    return this.http.post<ApplyResult>(`${environment.apiUrl}/portal/jobs/${jobId}/apply`, form);
  }
}
