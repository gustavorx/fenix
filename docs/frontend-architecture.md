# Frontend Architecture

This document records the initial frontend architecture decisions for Fenix before app scaffolding begins. The goal is a modern React application that remains approachable to a backend developer learning contemporary frontend development.

The current execution mode is shipping-first:

- finish a coherent V1 quickly;
- keep the app good enough for portfolio presentation;
- avoid architecture decisions that exist only to support speculative UI complexity.

The concrete scope for that release lives in `docs/frontend-v1-scope.md`.

## Goals

- Build a real monthly finance application, not a sample UI.
- Keep the frontend separate from the ASP.NET Core API while living in the same repository.
- Learn core modern frontend concepts explicitly: components, routing, server state, forms, validation, styling, and API consumption.
- Avoid tooling that generates or hides important application code too early.
- Keep architecture simple and feature-oriented as the application grows.

## Project Location

The frontend application will live at:

```text
src/web
```

The repository shape becomes:

```text
src/
  api/
  web/
```

`src/api` remains responsible for authentication, authorization, business rules, persistence, validation, and API contracts. `src/web` is a browser client of that API.

## Application Type

Fenix will use a client-side React single-page application (SPA).

In development:

```text
Browser -> Vite React app -> ASP.NET Core API -> PostgreSQL
```

The API remains the authoritative backend. The React app manages presentation, navigation, local interaction state, frontend validation feedback, and consumption of backend data.

## Selected Stack

| Concern | Choice | Reason |
| --- | --- | --- |
| UI framework | React | Widely used, component-oriented, and valuable for full-stack learning |
| Language | TypeScript | Gives explicit contracts and a familiar typed development model |
| Build/dev tooling | Vite | Lightweight modern tooling for a separate SPA consuming an existing API |
| Routing | React Router | Standard client-side navigation and protected app layouts |
| Server state | TanStack Query | Handles API loading, caching, invalidation, errors, and mutation refreshes |
| Forms | React Hook Form | Manages form state and submission cleanly for CRUD workflows |
| Frontend validation | Zod | Typed client-side validation schemas for good user feedback |
| Styling | Tailwind CSS | Modern utility styling suitable for a tailored app design system |
| Interactive UI primitives | Selective shadcn/ui components | Accessible complex controls without outsourcing the Fenix interface design |
| Package manager | npm | Standard, sufficient, and introduces no extra tooling overhead |

## Intentionally Not Selected

### Next.js

Fenix already has an ASP.NET Core backend and needs a browser client that consumes it. A full-stack React framework would introduce server-side React concepts and another backend boundary before they provide clear product value.

### Redux Or Zustand

No separate global client state library is needed initially. TanStack Query owns server-backed state, and React state is sufficient for local UI state such as selected month, dialogs, tabs, and input interactions.

### Axios

The frontend will use native `fetch` behind a small local API client. This keeps HTTP behavior visible and avoids another dependency where browser APIs are sufficient.

### Generated API Types Or Client Code

TypeScript request and response types will be handwritten intentionally. This makes the frontend/backend contract explicit during learning and keeps the API integration understandable.

When backend response contracts change, the corresponding frontend types and client calls must be reviewed manually.

## Development API Integration

During local development:

```text
Frontend: http://localhost:5173
API:      http://localhost:5207
```

Vite will proxy API requests:

```text
/api -> http://localhost:5207
```

Frontend code should request relative API paths:

```ts
fetch("/api/monthly-summary?month=4&year=2026", {
  credentials: "include",
});
```

Benefits:

- cookie-backed authentication remains straightforward;
- frontend code does not hardcode the local API host;
- the initial app does not require local CORS configuration;
- requests resemble a same-origin deployment model.

The final production hosting model is intentionally postponed until the app foundation exists.

## API Client Approach

The app will use a small handwritten wrapper around `fetch` under:

```text
src/web/src/shared/api/
```

The API client will centralize:

- `/api` request paths;
- `credentials: "include"` for auth cookie transport;
- JSON serialization and response parsing;
- mapping of the shared backend error contract:

```json
{
  "errors": [
    {
      "code": "some.code",
      "message": "Readable message.",
      "type": 0
    }
  ]
}
```

Feature-specific query and mutation functions remain close to their owning features.

## State Ownership

The frontend will distinguish two kinds of state.

### Server State

Data obtained from the API, such as:

- authenticated user;
- monthly summary;
- monthly expenses;
- monthly incomes;
- cards;
- people;
- shared receivables.

TanStack Query will manage server state, including loading, errors, caching, refetching, and invalidating data after mutations.

### UI State

Data that exists only to drive the current interface, such as:

- selected month;
- open or closed dialog;
- selected tab;
- active filters;
- form presentation state.

React state and form libraries will manage UI state.

## Styling And UI Components

Tailwind CSS provides the styling foundation and design-token implementation.

The first shared tokens, component families, and visual rules are documented in `docs/frontend-design-system.md`.

shadcn/ui will be used selectively for controls whose interaction and accessibility details are costly to rebuild correctly, such as:

- dialogs;
- confirmation dialogs;
- select/menu controls;
- tooltips;
- toasts;
- possibly complex form primitives.

Fenix-specific components will be designed and built locally, including:

- dashboard layout;
- monthly summary metrics;
- money displays;
- finance tables/lists;
- card-group expense sections;
- month selector composition;
- loading, empty, and error states.

The app should not become a generic shadcn assembly. Shared primitives support the interface; product design remains specific to the finance workflow.

For V1, prefer the simplest product-specific composition that can ship:

- explicit monthly sections over clever hidden panels;
- ordinary dialogs over unusual interaction patterns;
- one strong monthly page over many partially built surfaces.

## Source Structure

The source tree will use a feature-first organization:

```text
src/web/
  src/
    app/
      layouts/
      providers/
      router/
      styles/
    features/
      auth/
      dashboard/
      incomes/
      expenses/
      cards/
      people/
      shared-expenses/
    shared/
      api/
      lib/
      types/
      ui/
```

### `app`

Application-level composition:

- application entrypoint;
- global providers;
- router definitions;
- authenticated and unauthenticated layouts;
- global styles and design tokens.

### `features`

User workflows and domain-oriented screens. Code should stay with its feature when it is not reused outside that workflow.

Each feature starts small. Subfolders such as `api`, `components`, `pages`, or `schemas` should be introduced only when the feature has enough code to benefit from them.

### `shared/api`

Infrastructure shared across feature API calls:

- base fetch wrapper;
- shared error typing;
- request helpers;
- auth credential conventions.

### `shared/types`

Handwritten API DTO types shared by multiple features.

Types used by only one feature may initially remain inside that feature rather than becoming global.

### `shared/ui`

Reusable presentational and interactive controls:

- local design-system primitives;
- selectively added shadcn/ui components;
- generic feedback states when reused broadly.

### `shared/lib`

Pure reusable helpers, such as:

- money formatting;
- date formatting;
- month/year utilities;
- class-name utilities when required by UI tooling.

## Initial Workflow Sequence

After the architecture and design-system documentation are complete, implementation should proceed in this order:

1. Scaffold `src/web` with React, TypeScript, and Vite.
2. Configure Tailwind, routing, TanStack Query, and app layouts.
3. Implement authentication using cookie-backed API requests.
4. Apply the frontend-oriented API adjustments required for dashboard fidelity.
5. Build the monthly dashboard as the primary product surface.
6. Add only the management workflows required for real monthly use.

## Shipping-First V1 Notes

The architecture should support a portfolio-ready release, not endless UI exploration.

Implications:

- keep route count low at first;
- make the monthly dashboard the center of the app;
- use modal CRUD where it reduces navigation cost;
- defer broad abstraction until repeated usage proves it helpful;
- treat the more ambitious preview variants as reference material, not binding architecture.

## Deferred Decisions

- Production hosting/deployment topology.
- Whether frontend API types should ever become generated.
- Additional global state management, if real needs emerge.
- Advanced browser test tooling and breadth of component testing.
- Whether fixed expenses should become a first-class backend/domain concept.
