import { Component, OnInit, inject } from '@angular/core';
import { NgClass } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CandidateService } from '../../core/services/candidate.service';
import { LookupService } from '../../core/services/lookup.service';
import { Candidate } from '../../core/models/candidate.model';
import { LookupValue } from '../../core/models/lookup.model';

@Component({
  selector: 'app-candidates',
  standalone: true,
  imports: [NgClass, FormsModule, ReactiveFormsModule],
  templateUrl: './candidates.component.html',
})
export class CandidatesComponent implements OnInit {
  private readonly candidateService = inject(CandidateService);
  private readonly lookupService = inject(LookupService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  search = '';
  candidates: Candidate[] = [];
  statuses: LookupValue[] = [];
  loading = true;
  error = '';

  // Add candidate modal
  showAddModal = false;
  adding = false;
  addError = '';

  addForm = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName:  ['', [Validators.required, Validators.minLength(2)]],
    email:     ['', [Validators.required, Validators.email]],
    phone:     [''],
  });

  get f() { return this.addForm.controls; }

  ngOnInit(): void {
    this.lookupService.getByCategory('CandidateStatus').subscribe({
      next: (vals) => (this.statuses = vals),
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

  navigateTo(c: Candidate): void {
    this.router.navigate(['/candidates', c.id], { state: { candidate: c } });
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

  addCandidate(): void {
    if (this.addForm.invalid) { this.addForm.markAllAsTouched(); return; }
    this.adding = true;
    this.addError = '';
    const { firstName, lastName, email, phone } = this.addForm.getRawValue();
    this.candidateService.create({
      firstName: firstName!,
      lastName:  lastName!,
      email:     email!,
      phone:     phone || undefined,
    }).subscribe({
      next: (c) => {
        this.candidates = [c, ...this.candidates];
        this.closeModal();
        this.adding = false;
      },
      error: (err) => {
        this.addError = err?.error?.error ?? 'Failed to add candidate.';
        this.adding = false;
      },
    });
  }

  closeModal(): void {
    this.showAddModal = false;
    this.addError = '';
    this.addForm.reset();
  }
}
