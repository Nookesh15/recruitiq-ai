import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { JobPosting, CreateJobPostingRequest } from '../models/job-posting.model';
import { PaginatedList } from '../models/candidate.model';

@Injectable({ providedIn: 'root' })
export class JobPostingService {
  private readonly http = inject(HttpClient);

  getAll(page = 1, pageSize = 20): Observable<PaginatedList<JobPosting>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PaginatedList<JobPosting>>(`${environment.apiUrl}/jobpostings`, { params });
  }

  getById(id: string): Observable<JobPosting> {
    return this.http.get<JobPosting>(`${environment.apiUrl}/jobpostings/${id}`);
  }

  create(request: CreateJobPostingRequest): Observable<JobPosting> {
    return this.http.post<JobPosting>(`${environment.apiUrl}/jobpostings`, request);
  }

  updateStatus(id: string, status: string): Observable<JobPosting> {
    return this.http.patch<JobPosting>(`${environment.apiUrl}/jobpostings/${id}/status`, { status });
  }
}
