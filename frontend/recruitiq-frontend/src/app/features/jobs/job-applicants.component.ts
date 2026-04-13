import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgClass, DatePipe } from '@angular/common';
import { JobApplicationService } from '../../core/services/job-application.service';
import { JobPostingService } from '../../core/services/job-posting.service';
import { JobApplication } from '../../core/models/job-application.model';
import { JobPosting } from '../../core/models/job-posting.model';

@Component({
  selector: 'app-job-applicants',
  standalone: true,
  imports: [NgClass, RouterLink, DatePipe],
  templateUrl: './job-applicants.component.html',
})
export class JobApplicantsComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly appService = inject(JobApplicationService);
  private readonly jobService = inject(JobPostingService);

  job: JobPosting | null = null;
  applicants: JobApplication[] = [];
  loading = true;
  error = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;

    this.jobService.getById(id).subscribe({
      next: (j) => (this.job = j),
    });

    this.appService.getByJobId(id).subscribe({
      next: (apps) => {
        this.applicants = apps;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load applicants';
        this.loading = false;
      },
    });
  }

  scoreBarColor(score: number): string {
    if (score >= 80) return 'bg-green-500';
    if (score >= 60) return 'bg-indigo-500';
    return 'bg-orange-400';
  }

  scoreTextColor(score: number): string {
    if (score >= 80) return 'text-green-600';
    if (score >= 60) return 'text-indigo-600';
    return 'text-orange-500';
  }

  rank(index: number): string {
    if (index === 0) return '🥇';
    if (index === 1) return '🥈';
    if (index === 2) return '🥉';
    return `#${index + 1}`;
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  copyApplyLink(): void {
    const url = `${window.location.origin}/apply/${this.job?.id}`;
    navigator.clipboard.writeText(url);
  }
}
