import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject, switchMap } from 'rxjs';
import { JobPostingService } from '../../core/services/job-posting.service';
import { LookupService } from '../../core/services/lookup.service';
import { AiService, BiasFlag } from '../../core/services/ai.service';
import { JobPosting } from '../../core/models/job-posting.model';
import { LookupValue } from '../../core/models/lookup.model';

@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [NgClass, FormsModule],
  templateUrl: './jobs.component.html',
})
export class JobsComponent implements OnInit {
  private readonly jobService = inject(JobPostingService);
  private readonly lookupService = inject(LookupService);
  private readonly aiService = inject(AiService);
  private readonly router = inject(Router);
  private readonly jdChange$ = new Subject<string>();

  jobs: JobPosting[] = [];
  departments: LookupValue[] = [];
  loading = true;
  error = '';

  showAddModal = false;
  adding = false;
  addError = '';
  newTitle = '';
  newDepartment = '';
  newLocation = '';
  newDescription = '';
  biasFlags: BiasFlag[] = [];
  biasDismissed = false;

  ngOnInit(): void {
    this.lookupService.getByCategory('Department').subscribe(d => (this.departments = d));

    // Debounced bias check as description is typed
    this.jdChange$.pipe(
      debounceTime(700),
      distinctUntilChanged(),
      switchMap(text => this.aiService.analyzeJd(text)),
    ).subscribe({
      next: (result) => { this.biasFlags = result.biasFlags; this.biasDismissed = false; },
      error: () => { this.biasFlags = []; },
    });
    this.jobService.getAll().subscribe({
      next: (p) => {
        this.jobs = p.items;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load jobs';
        this.loading = false;
      },
    });
  }

  getStatusColor(status: string): string {
    const map: Record<string, string> = {
      Draft: 'bg-gray-100 text-gray-600',
      Active: 'bg-green-100 text-green-700',
      Paused: 'bg-yellow-100 text-yellow-700',
      Closed: 'bg-red-100 text-red-700',
    };
    return map[status] ?? 'bg-gray-100 text-gray-600';
  }

  createJob(): void {
    this.adding = true;
    this.addError = '';
    this.jobService.create({
      title: this.newTitle,
      description: this.newDescription,
      department: this.newDepartment,
      location: this.newLocation,
    }).subscribe({
      next: (job) => {
        this.jobs = [job, ...this.jobs];
        this.closeModal();
        this.adding = false;
      },
      error: (err) => {
        this.addError = err?.error?.error ?? 'Failed to create job.';
        this.adding = false;
      },
    });
  }

  updateStatus(job: JobPosting, status: string): void {
    this.jobService.updateStatus(job.id, status).subscribe({
      next: (updated) => {
        const idx = this.jobs.findIndex(j => j.id === job.id);
        if (idx !== -1) this.jobs[idx] = updated;
      },
    });
  }

  viewApplicants(job: JobPosting): void {
    this.router.navigate(['/jobs', job.id, 'applicants']);
  }

  copyApplyLink(job: JobPosting): void {
    const url = `${window.location.origin}/apply/${job.id}`;
    navigator.clipboard.writeText(url);
  }

  onDescriptionChange(value: string): void {
    if (value.trim().length >= 20) this.jdChange$.next(value);
    else this.biasFlags = [];
  }

  closeModal(): void {
    this.showAddModal = false;
    this.addError = '';
    this.biasFlags = [];
    this.biasDismissed = false;
    this.newTitle = this.newDepartment = this.newLocation = this.newDescription = '';
  }
}
