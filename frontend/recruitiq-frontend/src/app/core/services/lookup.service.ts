import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LookupValue } from '../models/lookup.model';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private readonly http = inject(HttpClient);
  private readonly cache = new Map<string, Observable<LookupValue[]>>();

  getByCategory(category: string): Observable<LookupValue[]> {
    if (!this.cache.has(category)) {
      const req$ = this.http
        .get<LookupValue[]>(`${environment.apiUrl}/lookups/${category}`)
        .pipe(shareReplay(1));
      this.cache.set(category, req$);
    }
    return this.cache.get(category)!;
  }
}
