import { Component, OnInit, inject } from '@angular/core';
import { NgClass, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService, DashboardStats } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [NgClass, RouterLink, DatePipe],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit {
  private readonly dashboardService = inject(DashboardService);

  stats: DashboardStats | null = null;
  loading = true;
  error = '';

  private readonly pipelineColors: Record<string, string> = {
    Applied:   'bg-gray-400',
    Screening: 'bg-blue-400',
    Interview: 'bg-indigo-400',
    Offer:     'bg-purple-400',
    Hired:     'bg-green-400',
    Rejected:  'bg-red-400',
  };

  private readonly statusBadge: Record<string, string> = {
    Applied:   'bg-gray-100 text-gray-700',
    Screening: 'bg-blue-100 text-blue-700',
    Interview: 'bg-indigo-100 text-indigo-700',
    Offer:     'bg-purple-100 text-purple-700',
    Hired:     'bg-green-100 text-green-700',
    Rejected:  'bg-red-100 text-red-700',
  };

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (s) => { this.stats = s; this.loading = false; },
      error: () => { this.error = 'Failed to load dashboard'; this.loading = false; },
    });
  }

  get maxPipeline(): number {
    return Math.max(...(this.stats?.pipeline.map(p => p.count) ?? [1]), 1);
  }

  pipelineColor(status: string): string {
    return this.pipelineColors[status] ?? 'bg-gray-400';
  }

  scoreColor(score: number): string {
    if (score >= 80) return 'text-green-600 font-bold';
    if (score >= 60) return 'text-indigo-600 font-bold';
    return 'text-orange-500 font-bold';
  }

  badgeClass(status: string): string {
    return this.statusBadge[status] ?? 'bg-gray-100 text-gray-700';
  }
}
