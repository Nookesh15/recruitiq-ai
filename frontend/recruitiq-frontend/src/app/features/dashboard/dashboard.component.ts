import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CandidateService } from '../../core/services/candidate.service';
import { LookupService } from '../../core/services/lookup.service';
import { Candidate } from '../../core/models/candidate.model';
import { LookupValue } from '../../core/models/lookup.model';

interface PipelineStage {
  stage: string;
  count: number;
  color: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgClass, RouterLink],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  private readonly candidateService = inject(CandidateService);
  private readonly lookupService = inject(LookupService);

  stats = [
    { label: 'Total Candidates', value: '—', change: 'Loading...', icon: '👤', color: 'bg-blue-500' },
    { label: 'Active Jobs', value: '14', change: '+3 new this week', icon: '💼', color: 'bg-indigo-500' },
    { label: 'Interviews Today', value: '7', change: '2 pending confirmation', icon: '📅', color: 'bg-purple-500' },
    { label: 'Avg AI Score', value: '—', change: 'Calculating...', icon: '🤖', color: 'bg-green-500' },
  ];

  recentCandidates: Candidate[] = [];
  statuses: LookupValue[] = [];
  pipeline: PipelineStage[] = [];
  loading = true;

  private readonly pipelineColors: Record<string, string> = {
    Applied: 'bg-gray-400',
    Screening: 'bg-blue-400',
    Interview: 'bg-indigo-400',
    Offer: 'bg-purple-400',
    Hired: 'bg-green-400',
    Rejected: 'bg-red-400',
  };

  ngOnInit(): void {
    this.lookupService.getByCategory('CandidateStatus').subscribe({
      next: (vals) => (this.statuses = vals),
    });

    this.candidateService.getAll(1, 100).subscribe({
      next: (page) => {
        this.recentCandidates = page.items.slice(0, 5);

        // Update stats
        const withScore = page.items.filter((c) => c.aiScore != null);
        const avgScore = withScore.length
          ? Math.round(withScore.reduce((s, c) => s + (c.aiScore ?? 0), 0) / withScore.length)
          : 0;

        this.stats[0].value = String(page.totalCount);
        this.stats[0].change = `${page.items.length} loaded`;
        this.stats[3].value = withScore.length ? `${avgScore}%` : 'N/A';
        this.stats[3].change = withScore.length ? `Based on ${withScore.length} scored` : 'No scores yet';

        // Build pipeline from real data
        const counts: Record<string, number> = {};
        for (const c of page.items) {
          counts[c.status] = (counts[c.status] ?? 0) + 1;
        }
        this.pipeline = Object.entries(counts).map(([stage, count]) => ({
          stage,
          count,
          color: this.pipelineColors[stage] ?? 'bg-gray-400',
        }));

        this.loading = false;
      },
      error: () => (this.loading = false),
    });
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

  getScoreColor(score: number): string {
    if (score >= 90) return 'text-green-600';
    if (score >= 75) return 'text-indigo-600';
    return 'text-orange-500';
  }

  getInitials(c: Candidate): string {
    return `${c.firstName[0]}${c.lastName[0]}`.toUpperCase();
  }

  get maxPipelineCount(): number {
    return Math.max(...this.pipeline.map((p) => p.count), 1);
  }
}
