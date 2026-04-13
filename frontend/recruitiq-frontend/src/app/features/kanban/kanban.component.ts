import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CdkDragDrop, CdkDrag, CdkDropList, CdkDropListGroup, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobPostingService } from '../../core/services/job-posting.service';
import { JobApplication } from '../../core/models/job-application.model';
import { JobPosting } from '../../core/models/job-posting.model';

export const STAGES = ['Applied', 'Screening', 'Interview', 'Offer', 'Hired', 'Rejected'] as const;
export type Stage = typeof STAGES[number];

@Component({
  selector: 'app-kanban',
  standalone: true,
  imports: [NgClass, FormsModule, RouterLink, CdkDrag, CdkDropList, CdkDropListGroup],
  templateUrl: './kanban.component.html',
})
export class KanbanComponent implements OnInit {
  private readonly appService = inject(JobApplicationService);
  private readonly jobService = inject(JobPostingService);

  readonly stages = STAGES;
  columns: Record<string, JobApplication[]> = {};
  jobs: JobPosting[] = [];
  selectedJobId = '';
  loading = true;
  error = '';

  ngOnInit(): void {
    // Init empty columns
    this.stages.forEach(s => (this.columns[s] = []));

    this.jobService.getAll().subscribe({ next: (p) => (this.jobs = p.items) });
    this.loadKanban();
  }

  loadKanban(): void {
    this.loading = true;
    this.stages.forEach(s => (this.columns[s] = []));

    this.appService.getKanban(this.selectedJobId || undefined).subscribe({
      next: (apps) => {
        apps.forEach(a => {
          const stage = (a.stage ?? 'Applied') as Stage;
          if (this.columns[stage]) this.columns[stage].push(a);
          else this.columns['Applied'].push(a);
        });
        this.loading = false;
      },
      error: () => { this.error = 'Failed to load pipeline'; this.loading = false; },
    });
  }

  drop(event: CdkDragDrop<JobApplication[]>, targetStage: string): void {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      const app = event.previousContainer.data[event.previousIndex];
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
      // Persist stage change
      this.appService.updateStage(app.id, targetStage).subscribe({
        error: () => {
          // Rollback on failure
          transferArrayItem(event.container.data, event.previousContainer.data, event.currentIndex, event.previousIndex);
        },
      });
    }
  }

  stageColor(stage: string): string {
    const map: Record<string, string> = {
      Applied:   'bg-gray-100 text-gray-600 border-gray-200',
      Screening: 'bg-blue-50 text-blue-700 border-blue-200',
      Interview: 'bg-indigo-50 text-indigo-700 border-indigo-200',
      Offer:     'bg-purple-50 text-purple-700 border-purple-200',
      Hired:     'bg-green-50 text-green-700 border-green-200',
      Rejected:  'bg-red-50 text-red-600 border-red-200',
    };
    return map[stage] ?? 'bg-gray-100 text-gray-600 border-gray-200';
  }

  stageHeaderColor(stage: string): string {
    const map: Record<string, string> = {
      Applied:   'bg-gray-500',
      Screening: 'bg-blue-500',
      Interview: 'bg-indigo-500',
      Offer:     'bg-purple-500',
      Hired:     'bg-green-500',
      Rejected:  'bg-red-400',
    };
    return map[stage] ?? 'bg-gray-400';
  }

  scoreColor(score: number): string {
    if (score >= 80) return 'text-green-600';
    if (score >= 60) return 'text-indigo-600';
    return 'text-orange-500';
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  daysSince(dateStr: string): number {
    return Math.floor((Date.now() - new Date(dateStr).getTime()) / 86_400_000);
  }

  totalCount(): number {
    return this.stages.reduce((sum, s) => sum + (this.columns[s]?.length ?? 0), 0);
  }
}
