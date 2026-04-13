import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgClass } from '@angular/common';
import { PortalService, PublicJob, ApplyResult } from '../../core/services/portal.service';

@Component({
  selector: 'app-apply',
  standalone: true,
  imports: [FormsModule, NgClass],
  templateUrl: './apply.component.html',
})
export class ApplyComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly portalService = inject(PortalService);

  job: PublicJob | null = null;
  result: ApplyResult | null = null;
  loadingJob = true;
  submitting = false;
  jobError = '';
  submitError = '';

  // Form fields
  firstName = '';
  lastName = '';
  email = '';
  phone = '';
  coverNote = '';
  resumeFile: File | null = null;

  ngOnInit(): void {
    const jobId = this.route.snapshot.paramMap.get('jobId')!;
    this.portalService.getJob(jobId).subscribe({
      next: (j) => { this.job = j; this.loadingJob = false; },
      error: () => { this.jobError = 'This job posting could not be found or is no longer available.'; this.loadingJob = false; },
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.resumeFile = input.files?.[0] ?? null;
  }

  get canSubmit(): boolean {
    return !!this.firstName && !!this.lastName && !!this.email && !!this.resumeFile && !this.submitting;
  }

  submit(): void {
    if (!this.canSubmit || !this.job) return;

    this.submitting = true;
    this.submitError = '';

    const form = new FormData();
    form.append('firstName', this.firstName);
    form.append('lastName', this.lastName);
    form.append('email', this.email);
    if (this.phone) form.append('phone', this.phone);
    if (this.coverNote) form.append('coverNote', this.coverNote);
    form.append('resume', this.resumeFile!);

    this.portalService.apply(this.job.id, form).subscribe({
      next: (res) => { this.result = res; this.submitting = false; },
      error: (err) => {
        this.submitError = err?.error?.error ?? 'Failed to submit application. Please try again.';
        this.submitting = false;
      },
    });
  }
}
