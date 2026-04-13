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
  // RIQAI-19: AI match reasoning
  matchReason?: string;
  strengths?: string[];
  gaps?: string[];
  // RIQAI-23: Kanban stage
  stage?: string;
}
