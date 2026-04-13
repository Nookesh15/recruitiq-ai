export interface ParsedExperience {
  role: string;
  company: string;
  duration: string;
}

export interface ParsedEducation {
  degree: string;
  field?: string;
  institution?: string;
  year?: string;
}

export interface Candidate {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  resumeUrl?: string;
  status: string;   // code from LookupValues (e.g. "Applied")
  aiScore?: number;
  createdAt: string;
  // RIQAI-18/21: parsed resume sections
  skills?: string[];
  experience?: ParsedExperience[];
  education?: ParsedEducation[];
  parsedSummary?: string;
}

export interface PaginatedList<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateCandidateRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
}
