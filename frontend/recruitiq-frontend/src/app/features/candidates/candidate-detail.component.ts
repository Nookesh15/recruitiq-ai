import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgClass, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CandidateService } from '../../core/services/candidate.service';
import { LookupService } from '../../core/services/lookup.service';
import { JobPostingService } from '../../core/services/job-posting.service';
import { JobApplicationService } from '../../core/services/job-application.service';
import { Candidate } from '../../core/models/candidate.model';
import { LookupValue } from '../../core/models/lookup.model';
import { JobPosting } from '../../core/models/job-posting.model';
import { JobApplication } from '../../core/models/job-application.model';

@Component({
  selector: 'app-candidate-detail',
  standalone: true,
  imports: [NgClass, FormsModule, RouterLink, DatePipe],
  templateUrl: './candidate-detail.component.html',
})
export class CandidateDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly candidateService = inject(CandidateService);
  private readonly lookupService = inject(LookupService);
  private readonly jobService = inject(JobPostingService);
  private readonly applicationService = inject(JobApplicationService);

  candidate: Candidate | null = null;
  statuses: LookupValue[] = [];
  applications: JobApplication[] = [];
  jobs: JobPosting[] = [];
  loading = true;
  error = '';

  activeTab: 'resume' | 'applications' = 'resume';

  // Resume upload
  uploadingResume = false;
  resumeFile: File | null = null;
  uploadMsg = '';

  // Apply to job modal
  showApplyModal = false;
  selectedJobId = '';
  applyNotes = '';
  applying = false;
  applyError = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;

    // Show candidate instantly from router navigation state (passed from the list)
    const navState = history.state as { candidate?: ReturnType<typeof Object.assign> };
    if (navState?.candidate) {
      this.candidate = navState.candidate as Candidate;
      this.loading = false;
    }

    this.lookupService.getByCategory('CandidateStatus').subscribe(v => (this.statuses = v));

    this.candidateService.getById(id).subscribe({
      next: (c) => {
        this.candidate = c;
        this.loading = false;
      },
      error: () => {
        if (!this.candidate) {
          this.error = 'Candidate not found.';
        }
        this.loading = false;
      },
    });

    this.applicationService.getByCandidateId(id).subscribe({
      next: (apps) => (this.applications = apps),
    });

    this.jobService.getAll(1, 100).subscribe({
      next: (p) => (this.jobs = p.items),
    });
  }

  getStatusLabel(code: string): string {
    return this.statuses.find(s => s.code === code)?.displayName ?? code;
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

  getInitials(): string {
    if (!this.candidate) return '';
    return `${this.candidate.firstName[0]}${this.candidate.lastName[0]}`.toUpperCase();
  }

  changeStatus(status: string): void {
    if (!this.candidate) return;
    this.candidateService.updateStatus(this.candidate.id, status).subscribe({
      next: (updated) => (this.candidate = updated),
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.resumeFile = input.files?.[0] ?? null;
  }

  uploadResume(): void {
    if (!this.candidate || !this.resumeFile) return;
    this.uploadingResume = true;
    this.uploadMsg = '';
    this.candidateService.uploadResume(this.candidate.id, this.resumeFile).subscribe({
      next: (updated) => {
        this.candidate = updated;
        this.uploadingResume = false;
        this.uploadMsg = 'Resume uploaded and AI scoring complete.';
        this.resumeFile = null;
      },
      error: () => {
        this.uploadingResume = false;
        this.uploadMsg = 'Upload failed.';
      },
    });
  }

  applyToJob(): void {
    if (!this.candidate || !this.selectedJobId) return;
    this.applying = true;
    this.applyError = '';
    this.applicationService.create(this.candidate.id, this.selectedJobId, this.applyNotes || undefined).subscribe({
      next: (app) => {
        this.applications = [app, ...this.applications];
        this.showApplyModal = false;
        this.selectedJobId = '';
        this.applyNotes = '';
        this.applying = false;
      },
      error: (err) => {
        this.applyError = err?.error?.error ?? 'Failed to apply.';
        this.applying = false;
      },
    });
  }

  deleteCandidate(): void {
    if (!this.candidate || !confirm('Delete this candidate? This cannot be undone.')) return;
    this.candidateService.delete(this.candidate.id).subscribe({
      next: () => this.router.navigate(['/candidates']),
    });
  }
}
