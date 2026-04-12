export interface JobPosting {
  id: string;
  title: string;
  description: string;
  department: string;
  location: string;
  status: string;
  applicationCount: number;
  createdAt: string;
}

export interface CreateJobPostingRequest {
  title: string;
  description: string;
  department: string;
  location: string;
}
