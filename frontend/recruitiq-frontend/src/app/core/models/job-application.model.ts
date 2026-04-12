export interface JobApplication {
  id: string;
  candidateId: string;
  candidateName: string;
  candidateEmail: string;
  jobPostingId: string;
  jobTitle: string;
  matchScore?: number;
  notes?: string;
  createdAt: string;
}
