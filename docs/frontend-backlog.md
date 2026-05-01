# Frontend Backlog

This backlog tracks the transition from the finished API-first backend work into a modern frontend for Fenix.

The first goal is not to code screens immediately. The first goal is to make the current API and product workflow explicit enough that the frontend can be implemented deliberately, with a small design system and a clear app architecture.

## Current Execution Mode

The backlog remains useful as reference, but the project is currently not being executed as a slow item-by-item discovery track.

The active direction is a shipping-first V1 for personal use and portfolio quality:

- `docs/frontend-v1-scope.md`

That means the practical build path compresses several backlog items into one focused release effort:

- scaffold the app;
- implement auth;
- ship the monthly dashboard;
- add the minimum mutation flows required for real monthly use.

The implementation sequence is intentionally incremental and end-to-end:

- keep the app working at every step;
- prefer small commits over broad frontend-only mock phases;
- implement auth before the dashboard read path;
- create folders only when code actually needs them.

## Summary

- [x] [0. Document Current API Contract](#0-document-current-api-contract)
- [x] [1. Capture Product Reference And Frontend Notes](#1-capture-product-reference-and-frontend-notes)
- [x] [2. Choose Frontend Stack And Project Structure](#2-choose-frontend-stack-and-project-structure)
- [x] [3. Define Initial Design System](#3-define-initial-design-system)
- [x] [4.0 Scaffold Frontend App Foundation](#40-scaffold-frontend-app-foundation)
- [x] [4.1 Add Router And Placeholder Pages](#41-add-router-and-placeholder-pages)
- [x] [4.2 Add Shared HTTP Client](#42-add-shared-http-client)
- [x] [4.3 Add Query Client Provider](#43-add-query-client-provider)
- [x] [5.1 Load Current Session](#51-load-current-session)
- [x] [5.2 Add Login Flow](#52-add-login-flow)
- [x] [5.3 Protect Home Route](#53-protect-home-route)
- [x] [5.4 Add Logout And Expired Session Handling](#54-add-logout-and-expired-session-handling)
- [ ] [6. Add Frontend-Oriented API Adjustments](#6-add-frontend-oriented-api-adjustments)
- [ ] [7. Build Monthly Dashboard](#7-build-monthly-dashboard)
- [ ] [8. Build Income Management](#8-build-income-management)
- [ ] [9. Build Expense Management](#9-build-expense-management)
- [ ] [10. Build Cards Management](#10-build-cards-management)
- [ ] [11. Build People And Shared Expenses Management](#11-build-people-and-shared-expenses-management)
- [ ] [12. Add Frontend Quality Gates](#12-add-frontend-quality-gates)

## 0. Document Current API Contract

Context

The backend API is ready enough to start frontend planning, but the frontend should not depend on reading controllers and use cases directly. The current API surface needs a clear contract that an agent or frontend developer can consume.

Motivation

OpenAPI can provide the generated technical contract, but the frontend also needs a curated product-facing guide that explains authentication, cookies, request conventions, response shapes, error handling, and which endpoints are intended for each workflow.

Next step

- Enable OpenAPI output in development.
- Verify whether the generated contract includes useful request and response schemas.
- Document the authentication model:
  - `POST /api/auth/login` creates the auth cookie.
  - `POST /api/auth/logout` clears the auth cookie.
  - `GET /api/auth/me` returns the current authenticated user.
  - Frontend requests must include credentials so the auth cookie is sent.
- Document the shared error response shape: `{ "errors": [...] }`.
- Create `docs/api-contract.md` as the human-readable API guide for frontend work.
- Group endpoints by workflow:
  - Auth
  - Monthly dashboard
  - Incomes
  - Expenses
  - Cards
  - People
  - Expense shares
  - Expense share installments

Done when

- The API exposes OpenAPI in development.
- `docs/api-contract.md` exists and describes the current endpoints, payloads, responses, auth behavior, and known product rules.

## 1. Capture Product Reference And Frontend Notes

Context

The current financial workflow exists in a spreadsheet. That spreadsheet is the best source of truth for the first frontend dashboard because it reflects the user's real monthly finance model.

Motivation

Before designing screens, the project needs to capture what the spreadsheet currently communicates: totals, categories, grouping, visible columns, monthly rhythm, and the mental model behind the manual workflow.

Next step

- Save a masked spreadsheet screenshot under `docs/references/`.
- Create `docs/frontend-product-notes.md`.
- Extract from the screenshot:
  - Key monthly totals.
  - Main table/list sections.
  - Required fields.
  - Terms and labels used by the user.
  - Which information must be visible on the first dashboard.
  - Which information can live in secondary screens.
- Record any product decisions that are not obvious from the backend contract.

Done when

- The masked spreadsheet reference is available.
- Product notes describe the first frontend version in user-facing terms.

## 2. Choose Frontend Stack And Project Structure

Context

The user wants to learn a modern frontend framework and has previous experience with vanilla HTML, CSS, and JavaScript.

Motivation

The stack should be modern enough to learn relevant patterns, but not so complex that framework decisions dominate the product work.

Proposed direction

- React
- TypeScript
- Vite
- React Router
- TanStack Query
- React Hook Form
- Zod
- Tailwind CSS
- shadcn/ui-inspired local components

Next step

- Confirm whether the frontend will live inside this repository.
- Confirm the frontend directory name, for example `src/web` or `src/frontend`.
- Decide whether API types will be generated from OpenAPI or maintained manually at first.
- Define the high-level source layout:
  - `app`
  - `features`
  - `shared/api`
  - `shared/ui`
  - `shared/lib`
  - `shared/types`

Done when

- The stack decision is recorded.
- The app directory and module structure are decided before scaffolding.

## 3. Define Initial Design System

Context

Fenix is a personal finance management app, so the UI should feel calm, clear, and useful for repeated monthly work. It should not feel like a marketing page.

Motivation

A small design system will keep the frontend consistent while the app grows feature by feature.

Next step

- Document the design system in `docs/frontend-design-system.md`.
- Define design principles:
  - Dashboard-first.
  - Dense enough for financial scanning.
  - Calm visual hierarchy.
  - Explicit positive and negative financial states.
  - Fast entry for recurring monthly tasks.
- Define tokens:
  - Backgrounds.
  - Surfaces.
  - Text colors.
  - Muted text.
  - Borders.
  - Income color.
  - Expense color.
  - Shared receivable color.
  - Warning/destructive color.
  - Radius.
  - Shadows.
  - Spacing scale.
  - Typography scale.
- Define component families:
  - Buttons.
  - Icon buttons.
  - Inputs.
  - Money input.
  - Date input.
  - Month selector.
  - Select.
  - Checkbox/toggle.
  - Form field.
  - Dialog.
  - Confirmation dialog.
  - Toast/notification.
  - Summary metric.
  - Data table/list.
  - Empty state.
  - Error state.
  - Loading state.

Done when

- A first design-system document exists.
- The first dashboard and forms can be implemented from shared tokens and components instead of one-off styles.

## 4.0 Scaffold Frontend App Foundation

Context

After the API contract, product notes, stack, and design system are defined, the frontend can be created safely.

Motivation

The foundation should support authenticated app workflows, typed API access, routing, query caching, and reusable UI from the start.

Next step

- Create the frontend project.
- Configure TypeScript.
- Configure formatting/linting.
- Leave the app in a clean neutral state after removing template/demo content.

Done when

- The frontend app runs locally.
- The template/demo content is removed.
- Basic quality commands run successfully.

## 4.1 Add Router And Placeholder Pages

Context

Before wiring real auth, the app should understand URLs and page boundaries. This keeps the first auth integration small and makes the app behavior easier to reason about.

Motivation

The first learning step after the scaffold should be navigation, not dashboard complexity.

Next step

- Install `react-router-dom`.
- Add a minimal router.
- Add a public `/login` page.
- Add a private-target placeholder `/` page that can later become the authenticated home/dashboard entry.
- Keep the pages visually simple.

Done when

- The app can navigate between `/login` and `/`.
- The route setup is clear enough to extend without restructuring.

## 4.2 Add Shared HTTP Client

Context

The frontend will talk to the API through cookie-backed requests. Raw `fetch` calls should not be repeated across features from the start.

Motivation

A small shared HTTP client keeps auth behavior, JSON handling, and error parsing consistent while the app is still small.

Next step

- Create a shared wrapper around `fetch`.
- Include `credentials: "include"` by default.
- Handle JSON request and response bodies.
- Parse the shared backend error shape `{ "errors": [...] }`.
- Keep it intentionally small and handwritten.

Done when

- Feature code can call one shared helper instead of repeating raw `fetch`.
- Cookie-backed requests work with the local API setup.

## 4.3 Add Query Client Provider

Context

The app needs a consistent way to load server state such as the current session and later dashboard data.

Motivation

TanStack Query should be introduced before auth reads so session loading, retry behavior, and future cache invalidation all sit on the same foundation.

Next step

- Install `@tanstack/react-query`.
- Create the shared `QueryClient`.
- Add the `QueryClientProvider` near the app root.
- Keep the first configuration simple.

Done when

- The app has a working query foundation for authenticated reads.
- Future auth and dashboard data can use the same query model.

## 5.1 Load Current Session

Context

The API uses JWT authentication with cookie token transport. The frontend should treat authentication as cookie-backed session behavior instead of manually storing tokens.

Motivation

Before submitting credentials, the app should already know how to ask the backend whether a valid session exists.

Next step

- Add the current-user request for `GET /api/auth/me`.
- Use the shared HTTP client.
- Use TanStack Query for loading and caching.
- Decide what the app should show while the session check is still loading.

Done when

- The app can detect whether the user is authenticated on refresh.
- Session state is available for route decisions.

## 5.2 Add Login Flow

Context

With routing, the HTTP client, and the session query in place, the login page can become real instead of placeholder UI.

Motivation

This is the first visible end-to-end flow and the first place where the frontend should show backend-driven validation/auth errors.

Next step

- Build the login form.
- Call `POST /api/auth/login`.
- Show invalid-credential feedback for `400` and `401` responses as appropriate.
- Refresh or invalidate the session query after successful login.
- Redirect to `/` after success.

Done when

- A user can log in from the real frontend.
- Invalid credentials produce a visible and understandable error state.

## 5.3 Protect Home Route

Context

The app needs a first private surface even before the monthly dashboard exists.

Motivation

A protected home route proves that auth works end-to-end before we add real finance screens.

Next step

- Protect `/` using the loaded session state.
- Redirect unauthenticated users to `/login`.
- Keep the authenticated home intentionally minimal for now.

Done when

- Unauthenticated users cannot stay on `/`.
- Authenticated users can reach a private home page.

## 5.4 Add Logout And Expired Session Handling

Context

Authentication is incomplete until the frontend can end the session intentionally and recover gracefully from expired or missing cookies.

Motivation

This closes the auth loop before the dashboard starts depending on it.

Next step

- Call `POST /api/auth/logout`.
- Clear or invalidate session state after logout.
- Redirect to `/login` after logout.
- Define how the app reacts to `401` from protected reads after a session expires.

Done when

- A user can log out cleanly.
- Expired or invalid sessions return the user to the login flow without exposing private screens.

## 6. Add Frontend-Oriented API Adjustments

Context

The spreadsheet reference includes small display details that make the first dashboard easier to scan. Most of the required API contract already exists, but a few frontend-oriented read model adjustments should happen before building the dashboard so the UI does not need awkward joins or hidden assumptions.

Motivation

The first dashboard should stay close to the spreadsheet. Shared receivables currently have a monthly endpoint, but the response does not expose installment progress like `4/10`, which appears in the spreadsheet and helps identify where a receivable sits in a longer shared purchase.

Next step

- Extend `GET /api/expense-share-installments/monthly` items with:
  - `installmentNumber`
  - `totalInstallments`
- Derive the progress from the installments that belong to the same `ExpenseShare`, ordered by `DueDate`, then `Id`.
- Keep the existing monthly shared receivable fields:
  - person
  - source expense
  - amount
  - due date
  - paid date
  - paid state
- Update `docs/api-contract.md` after changing the response shape.
- Rebuild and verify OpenAPI still exposes the updated response schema.

Done when

- Monthly shared receivable rows can render spreadsheet-style progress such as `4/10`.
- The OpenAPI contract and human-readable API guide include the new fields.

## 7. Build Monthly Dashboard

Context

The dashboard is the main product surface. It should replace the spreadsheet's monthly overview first.

Motivation

The monthly dashboard validates whether the frontend direction matches the user's real workflow.

Next step

- Add month/year selection.
- Fetch `GET /api/monthly-summary`.
- Fetch monthly expenses.
- Fetch monthly incomes.
- Fetch monthly shared receivables if needed for detail views.
- Render summary metrics:
  - Total incomes.
  - Total shared receivables.
  - Total gross expenses.
  - Total net expenses.
  - Final balance.
- Render monthly expense list/table.
- Show card information when available.
- Show share indicators when an expense has shares.
- Add loading, empty, and error states.

Done when

- The dashboard answers the user's main monthly finance questions without needing the spreadsheet.

## 8. Build Income Management

Context

The API supports creating, listing, reading, updating, deleting, and monthly querying incomes.

Motivation

Income management is a contained CRUD workflow and a good first feature after auth and dashboard read models.

Next step

- List monthly incomes.
- Create income.
- Edit income.
- Delete income with confirmation.
- Validate amount, description, and received date on the client.
- Refresh dashboard data after mutations.

Done when

- The user can manage monthly incomes fully from the frontend.

## 9. Build Expense Management

Context

Expenses model purchases and installments. Expense correction is currently delete and recreate; expense update is intentionally postponed.

Motivation

Expense creation is the most important and most complex frontend workflow.

Next step

- List monthly expenses.
- View expense detail.
- Create cash expense with one installment.
- Create installment expense with generated schedule.
- Create installment expense with explicit installment values.
- Support optional card association.
- Support optional shares during creation if the workflow is clear enough.
- Delete expense with confirmation.
- Make the delete-and-recreate correction workflow clear in the UI.

Done when

- The user can register and remove expenses in the same way the API models them.

## 10. Build Cards Management

Context

Cards are optional associations on expenses and help distinguish cash flow from card-based purchases.

Motivation

Card management supports more accurate expense classification and future statement-oriented views.

Next step

- List cards.
- Create card.
- Edit card.
- Delete card with confirmation.
- Use cards in expense creation.
- Display linked card identity in monthly expense views.

Done when

- Cards can be managed independently and used during expense creation.

## 11. Build People And Shared Expenses Management

Context

Expense shares belong to expenses and can have explicit share installments. People can be deleted, leaving shares orphaned.

Motivation

Shared expenses affect the monthly summary and are part of the spreadsheet replacement workflow.

Next step

- List people.
- Create person.
- Edit person.
- Delete person with confirmation and clear orphaning behavior.
- Add share to expense.
- Edit share person assignment.
- Delete share.
- Add share installment.
- Edit share installment amount, due date, and paid date behavior.
- Delete share installment.
- Show monthly shared receivables.

Done when

- The user can manage people and shared receivables without leaving the app.

## 12. Add Frontend Quality Gates

Context

The frontend should remain maintainable as a learning project and as a real app.

Motivation

Small quality gates prevent the app from becoming hard to change while the user is learning React.

Next step

- Add formatting.
- Add linting.
- Add TypeScript checks.
- Add focused component tests where useful.
- Add API-client tests or mocked query tests for critical flows.
- Add basic browser verification for:
  - Login.
  - Dashboard load.
  - Income create/edit/delete.
  - Expense create/delete.
- Document local run commands.

Done when

- The frontend has repeatable checks and a short manual verification path for core flows.
