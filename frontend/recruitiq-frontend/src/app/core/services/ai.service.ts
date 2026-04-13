import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface BiasFlag {
  phrase: string;
  category: string;
  suggestion: string;
}

export interface JdAnalysisResult {
  flagCount: number;
  biasFlags: BiasFlag[];
  isClean: boolean;
}

@Injectable({ providedIn: 'root' })
export class AiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiUrl}/ai`;

  analyzeJd(jdText: string): Observable<JdAnalysisResult> {
    return this.http.post<JdAnalysisResult>(`${this.base}/analyze-jd`, { jdText });
  }
}
