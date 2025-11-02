# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a React + TypeScript personal finance management application (Cuppie Finance Manager) built with Vite. The frontend connects to a backend API for user authentication and financial transaction management.

## Development Commands

### Core Development
- `npm run dev` - Start development server (default port: 3000)
- `npm run build` - Type-check with TypeScript and build production bundle
- `npm run preview` - Preview production build locally
- `npm run lint` - Lint TypeScript/TSX files with ESLint

### Styling
- `npm run css` - Watch and compile Tailwind CSS (input: src/styles/tailwind.css, output: src/styles/tailwindout.css)

### Type Checking
- `npx tsc --noEmit` - Run TypeScript type checking without emitting files (also runs as part of build)

## Architecture Overview

### Authentication & State Management

**AuthContext Pattern**: The application uses React Context API for global authentication state management ([src/context/AuthContext.tsx](src/context/AuthContext.tsx)). This is the single source of truth for user authentication state.

- `AuthProvider` wraps the entire app in [App.tsx](src/App.tsx)
- `useAuth()` hook provides access to authentication state and methods
- Authentication state includes: `user`, `isLoading`, `isAuthenticated`
- All auth methods (login, register, logout) are centralized in the context

**Protected Routes**: Routes requiring authentication are wrapped with `ProtectedRoute` component ([src/components/ProtectedRoute.tsx](src/components/ProtectedRoute.tsx)) which handles loading states and redirects unauthenticated users to `/auth`.

### API Architecture

**Centralized API Client**: All API communication goes through [src/lib/api.ts](src/lib/api.ts):

- Uses Axios with base configuration (credentials enabled, JSON content-type)
- Implements automatic token refresh via response interceptor
- Token refresh queue prevents multiple simultaneous refresh requests
- Base URL: `VITE_API_URL` env variable or fallback to `http://localhost:5295`
- API exports: `authApi` (auth endpoints), `pagesApi` (page endpoints)

**Token Refresh Strategy**: When a 401 response is received:
1. Request is queued if refresh is already in progress
2. Otherwise, triggers `/auth/refresh` endpoint
3. On success, retries original request
4. On failure, rejects all queued requests

### Component Structure

**Pages**: Main application views in [src/Pages/](src/Pages/)
- `AuthPage.tsx` - Login/registration forms
- `HomePage.tsx` - Main dashboard with transactions, categories, and analytics

**UI Components**: Shadcn/ui based components in [src/components/ui/](src/components/ui/)
- Built on Radix UI primitives
- Styled with Tailwind CSS using CSS variables for theming
- Utilities via `cn()` helper ([src/lib/utils.ts](src/lib/utils.ts)) for conditional class merging

**Feature Components**: Business logic components in [src/components/](src/components/)
- `TransactionFilter.tsx` - Multi-filter component for transactions (type, categories, date range)
- `ProtectedRoute.tsx` - Route guard for authenticated pages

### Data Flow Pattern

**HomePage Transaction Management**: [src/Pages/HomePage.tsx](src/Pages/HomePage.tsx) demonstrates the complete data flow pattern:

1. **Data Loading**: Uses `useEffect` hooks to load categories and transactions on mount and filter changes
2. **User Scoping**: All transaction API calls include `userId` to ensure data isolation
3. **Optimistic UI**: Loading states shown via `Skeleton` components during data fetches
4. **Error Handling**: Errors displayed via `Alert` component, can be dismissed by user
5. **State Synchronization**: After mutations (create/update/delete), data is reloaded to stay in sync

**Important**: The HomePage directly uses `fetch` API instead of the centralized axios client. This is an architectural inconsistency to be aware of when making changes.

### Validation & Types

**Zod Schemas**: Form validation defined in [src/lib/validation.ts](src/lib/validation.ts)
- `loginSchema` - Username and password validation
- `registerSchema` - Username, email, password with regex patterns

**TypeScript Types**: Type definitions in [src/types/auth.ts](src/types/auth.ts)
- `User`, `LoginData`, `RegisterData`, `AuthResponse`
- AuthResponse has flexible structure to handle different backend response formats

### Routing

**React Router v6**: Three main routes in [App.tsx](src/App.tsx):
- `/auth` - Public authentication page
- `/home` - Protected main dashboard
- `/` - Redirects to `/home`

### Styling Architecture

**Tailwind CSS**: Custom compiled process
- Source: [src/styles/tailwind.css](src/styles/tailwind.css)
- Output: [src/styles/tailwindout.css](src/styles/tailwindout.css) (compiled, imported in main.tsx)
- Config: [tailwind.config.js](tailwind.config.js) with CSS variable-based theming
- Use `npm run css` to watch and recompile during development

**Design System**: Custom color palette using HSL CSS variables (--primary, --secondary, --destructive, --muted, --accent, etc.) defined in theme.extend.colors

## Path Aliases

TypeScript and Vite are configured with `@/` alias pointing to `./src/`:
- Use `@/components/ui/button` instead of `../../../components/ui/button`
- Configured in both [tsconfig.json](tsconfig.json) and [vite.config.ts](vite.config.ts)

## API Backend Integration

**Backend URL Configuration**:
- Environment variable: `VITE_API_URL`
- Default fallback: `http://localhost:5295`
- Build-time injection via Docker ARG when containerized

**API Proxy** (Development): Vite dev server proxies `/api` requests to `http://api:5000` (Docker compose environment)

**Credential Handling**: All API requests use `credentials: 'include'` for cookie-based authentication (refresh/access tokens)

## Important Implementation Notes

### Multi-user Data Isolation
Transaction and category operations must always filter by `userId` to prevent data leakage between users. The HomePage demonstrates this pattern - always include user context in API calls.

### Category ID 1 Protection
Category with `id === 1` is protected from deletion (see HomePage.tsx:362). This is a business rule enforced in the UI.

### Flexible Auth Response Handling
The `AuthContext` handles multiple backend response formats:
- Nested user object: `response.data.user`
- Direct user data: `response.data` with `id` and `username` fields
- Fallback to `checkAuthStatus()` if user data not returned

### Number Formatting
Use the `formatNumber()` utility from HomePage for consistent thousand separators (space-separated) and decimal formatting.

## Docker Deployment

Multi-stage Dockerfile:
1. **Build stage**: Node 22 Alpine, npm install, compile with Vite
2. **Production stage**: Nginx Alpine serving static build
- Requires nginx.conf file for proper SPA routing
- Accepts `VITE_API_URL` build arg for API endpoint configuration
