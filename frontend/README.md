# Frontend — Angular 21

RecruitIQ AI web application built with Angular 21 and TailwindCSS.

## Prerequisites

- Node.js 20+
- npm 10+

## Setup

```bash
npm install
npm start          # Dev server → http://localhost:4200
npm run build      # Production build
npm run test       # Unit tests
npm run lint       # ESLint
```

## Environment Config

Copy `.env.example` to `.env` and fill in values:

```
API_URL=http://localhost:5000/api/v1
```

## Structure

```
src/
├── app/
│   ├── core/        # Singleton services, guards, interceptors
│   ├── shared/      # Reusable components, pipes, directives
│   ├── features/    # Lazy-loaded feature modules
│   └── layout/      # Shell, header, sidebar
└── environments/
```
