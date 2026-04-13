import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { JobApplication } from '../models/job-application.model';

@Injectable({ providedIn: 'root' })
export class JobApplicationService {
  private readonly http = inject(HttpClient);

  getByCandidateId(candidateId: string): Observable<JobApplication[]> {
    return this.http.get<JobApplication[]>(
      `${environment.apiUrl}/jobapplications/candidate/${candidateId}`
    );
  }

  getByJobId(jobId: string): Observable<JobApplication[]> {
    return this.http.get<JobApplication[]>(
      `${environment.apiUrl}/jobapplications/job/${jobId}`
    );
  }

  create(candidateId: string, jobPostingId: string, notes?: string): Observable<JobApplication> {
    return this.http.post<JobApplication>(`${environment.apiUrl}/jobapplications`, {
      candidateId,
      jobPostingId,
      notes,
    });
  }
}
