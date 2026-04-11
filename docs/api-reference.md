# API Reference

Base URL: `https://api.recruitiq.ai/api/v1`

All endpoints require `Authorization: Bearer <token>` unless marked **Public**.

---

## Auth

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/auth/register` | Register new user | Public |
| POST | `/auth/login` | Login, returns JWT | Public |
| POST | `/auth/refresh` | Refresh access token | Public |
| POST | `/auth/logout` | Invalidate refresh token | Auth |

---

## Candidates

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/candidates` | List all candidates (paginated) | Recruiter |
| GET | `/candidates/{id}` | Get candidate by ID | Recruiter |
| POST | `/candidates` | Create candidate profile | Recruiter |
| PUT | `/candidates/{id}` | Update candidate | Recruiter |
| DELETE | `/candidates/{id}` | Soft delete candidate | Admin |

---

## Job Postings

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/jobs` | List all job postings | Auth |
| GET | `/jobs/{id}` | Get job posting by ID | Auth |
| POST | `/jobs` | Create job posting | Recruiter |
| PUT | `/jobs/{id}` | Update job posting | Recruiter |
| DELETE | `/jobs/{id}` | Soft delete posting | Admin |

---

## Resumes

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/resumes/upload` | Upload PDF/DOCX resume | Recruiter |
| GET | `/resumes/{candidateId}` | Get resume for candidate | Recruiter |
| GET | `/resumes/{candidateId}/score` | Get AI score for candidate | Recruiter |

---

## Response Format

### Success
```json
{
  "success": true,
  "data": { },
  "message": null
}
```

### Error
```json
{
  "success": false,
  "data": null,
  "message": "Candidate not found",
  "errors": ["Field 'email' is required"]
}
```

### Pagination
```json
{
  "success": true,
  "data": {
    "items": [],
    "totalCount": 100,
    "page": 1,
    "pageSize": 20,
    "totalPages": 5
  }
}
```

---

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK |
| 201 | Created |
| 204 | No Content (delete) |
| 400 | Bad Request — validation error |
| 401 | Unauthorized — missing/invalid token |
| 403 | Forbidden — insufficient role |
| 404 | Not Found |
| 409 | Conflict — duplicate resource |
| 422 | Unprocessable Entity |
| 500 | Internal Server Error |
