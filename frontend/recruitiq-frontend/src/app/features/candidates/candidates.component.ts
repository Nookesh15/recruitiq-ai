import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CandidateService } from '../../core/services/candidate.service';
import { LookupService } from '../../core/services/lookup.service';
import { Candidate } from '../../core/models/candidate.model';
import { LookupValue } from '../../core/models/lookup.model';

@Component({
  selector: 'app-candidates',
  standalone: true,
  imports: [NgClass, FormsModule],
  templateUrl: './candidates.component.html',
})
export class CandidatesComponent implements OnInit {
  private readonly candidateService = inject(CandidateService);
  private readonly lookupService = inject(LookupService);

  search = '';
  candidates: Candidate[] = [];
  statuses: LookupValue[] = [];
  loading = true;
  error = '';

  ngOnInit(): void {
    this.lookupService.getByCategory('CandidateStatus').subscribe({
      next: (vals) => (this.statuses = vals),
      error: () => (this.error = 'Failed to load statuses'),
    });

    this.candidateService.getAll().subscribe({
      next: (page) => {
        this.candidates = page.items;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load candidates';
        this.loading = false;
      },
    });
  }

  get filtered(): Candidate[] {
    if (!this.search) return this.candidates;
    const s = this.search.toLowerCase();
    return this.candidates.filter(
      (c) =>
        `${c.firstName} ${c.lastName}`.toLowerCase().includes(s) ||
        c.email.toLowerCase().includes(s)
    );
  }

  getStatusLabel(code: string): string {
    return this.statuses.find((s) => s.code === code)?.displayName ?? code;
  }

  getStatusColor(code: string): string {
    const map: Record<string, string> = {
      Applied: 'bg-gray-100 text-gray-700',
      Screening: 'bg-blue-100 text-blue-700',
      Interview: 'bg-indigo-100 text-indigo-700',
      Offer: 'bg-purple-100 text-purple-700',
      Hired: 'bg-green-100 text-green-700',
      Rejected: 'bg-red-100 text-red-700',
    };
    return map[code] ?? 'bg-gray-100 text-gray-700';
  }

  getScoreBar(score: number): string {
    if (score >= 90) return 'bg-green-500';
    if (score >= 75) return 'bg-indigo-500';
    return 'bg-orange-400';
  }

  getInitials(c: Candidate): string {
    return `${c.firstName[0]}${c.lastName[0]}`.toUpperCase();
  }
}
