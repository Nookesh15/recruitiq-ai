import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LookupValue } from '../models/lookup.model';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private readonly http = inject(HttpClient);

  getByCategory(category: string): Observable<LookupValue[]> {
    return this.http.get<LookupValue[]>(`${environment.apiUrl}/lookups/${category}`);
  }
}
