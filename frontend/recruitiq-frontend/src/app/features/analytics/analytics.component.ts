import { Component, OnInit, AfterViewInit, OnDestroy, inject, ElementRef, ViewChild } from '@angular/core';
import { NgClass, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart, BarController, BarElement, CategoryScale, LinearScale, Tooltip, Legend, ArcElement, DoughnutController } from 'chart.js';
import { AnalyticsService, FunnelResult, StageFunnelItem } from '../../core/services/analytics.service';
import { JobPostingService } from '../../core/services/job-posting.service';
import { JobPosting } from '../../core/models/job-posting.model';

Chart.register(BarController, BarElement, CategoryScale, LinearScale, Tooltip, Legend, ArcElement, DoughnutController);

const STAGE_COLORS: Record<string, string> = {
  Applied:   '#6b7280',
  Screening: '#3b82f6',
  Interview: '#6366f1',
  Offer:     '#a855f7',
  Hired:     '#22c55e',
  Rejected:  '#f87171',
};

@Component({
  selector: 'app-analytics',
  standalone: true,
  imports: [NgClass, FormsModule, DecimalPipe],
  templateUrl: './analytics.component.html',
})
export class AnalyticsComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('funnelCanvas') funnelCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('donutCanvas') donutCanvas!: ElementRef<HTMLCanvasElement>;

  private readonly analyticsService = inject(AnalyticsService);
  private readonly jobService = inject(JobPostingService);
  private funnelChart: Chart | null = null;
  private donutChart: Chart | null = null;

  jobs: JobPosting[] = [];
  selectedJobId = '';
  data: FunnelResult | null = null;
  loading = true;
  error = '';
  chartsReady = false;

  ngOnInit(): void {
    this.jobService.getAll().subscribe({ next: (p) => (this.jobs = p.items) });
    this.load();
  }

  ngAfterViewInit(): void {
    this.chartsReady = true;
    if (this.data) this.renderCharts();
  }

  ngOnDestroy(): void {
    this.funnelChart?.destroy();
    this.donutChart?.destroy();
  }

  load(): void {
    this.loading = true;
    this.analyticsService.getFunnel(this.selectedJobId || undefined).subscribe({
      next: (d) => {
        this.data = d;
        this.loading = false;
        if (this.chartsReady) this.renderCharts();
      },
      error: () => { this.error = 'Failed to load analytics'; this.loading = false; },
    });
  }

  private renderCharts(): void {
    if (!this.data) return;
    this.renderFunnelChart(this.data.funnel);
    this.renderDonutChart(this.data.funnel);
  }

  private renderFunnelChart(funnel: StageFunnelItem[]): void {
    this.funnelChart?.destroy();
    const ctx = this.funnelCanvas.nativeElement.getContext('2d')!;
    this.funnelChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: funnel.map(f => f.stage),
        datasets: [{
          label: 'Candidates',
          data: funnel.map(f => f.count),
          backgroundColor: funnel.map(f => STAGE_COLORS[f.stage] ?? '#6b7280'),
          borderRadius: 6,
          borderSkipped: false,
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false }, tooltip: { callbacks: {
          label: (ctx) => ` ${ctx.parsed.y} candidate${ctx.parsed.y !== 1 ? 's' : ''}`,
        }}},
        scales: {
          y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: '#f3f4f6' } },
          x: { grid: { display: false } },
        },
      },
    });
  }

  private renderDonutChart(funnel: StageFunnelItem[]): void {
    this.donutChart?.destroy();
    const nonZero = funnel.filter(f => f.count > 0);
    if (!nonZero.length) return;
    const ctx = this.donutCanvas.nativeElement.getContext('2d')!;
    this.donutChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: nonZero.map(f => f.stage),
        datasets: [{
          data: nonZero.map(f => f.count),
          backgroundColor: nonZero.map(f => STAGE_COLORS[f.stage] ?? '#6b7280'),
          borderWidth: 2,
          borderColor: '#fff',
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { position: 'right', labels: { boxWidth: 12, font: { size: 12 } } } },
        cutout: '65%',
      },
    });
  }

  stageColor(stage: string): string {
    const map: Record<string, string> = {
      Applied:   'bg-gray-100 text-gray-600',
      Screening: 'bg-blue-100 text-blue-700',
      Interview: 'bg-indigo-100 text-indigo-700',
      Offer:     'bg-purple-100 text-purple-700',
      Hired:     'bg-green-100 text-green-700',
      Rejected:  'bg-red-100 text-red-600',
    };
    return map[stage] ?? 'bg-gray-100 text-gray-600';
  }
}
