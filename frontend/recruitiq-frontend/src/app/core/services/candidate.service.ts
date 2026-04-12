import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Candidate, PaginatedList, CreateCandidateRequest } from '../models/candidate.model';

@Injectable({ providedIn: 'root' })
export class CandidateService {
  private readonly http = inject(HttpClient);

  getAll(page = 1, pageSize = 20): Observable<PaginatedList<Candidate>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PaginatedList<Candidate>>(`${environment.apiUrl}/candidates`, { params });
  }

  getById(id: string): Observable<Candidate> {
    return this.http.get<Candidate>(`${environment.apiUrl}/candidates/${id}`);
  }

  create(request: CreateCandidateRequest): Observable<Candidate> {
    return this.http.post<Candidate>(`${environment.apiUrl}/candidates`, request);
  }

  updateStatus(id: string, status: string): Observable<Candidate> {
    return this.http.patch<Candidate>(`${environment.apiUrl}/candidates/${id}/status`, { status });
  }

  uploadResume(id: string, file: File): Observable<Candidate> {
    const form = new FormData();
    form.append('file', file);
    return this.http.post<Candidate>(`${environment.apiUrl}/candidates/${id}/resume`, form);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/candidates/${id}`);
  }
}
