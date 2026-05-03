# Agent Guidelines

This repository contains Fenix, a personal finance management API. Treat it as an ASP.NET Core Web API project targeting .NET 10 LTS, backed by Entity Framework Core and PostgreSQL.

## Project Context

- The solution is currently intentionally small: `fenix.sln` with the API project in `src/api`.
- The API uses controllers, feature-oriented use cases, EF Core, PostgreSQL, JWT authentication, cookie token transport, and OpenTelemetry-based observability.
- The product is under active development, so keep changes focused and avoid broad architectural rewrites unless the user asks for them.
- Use `docs/backlog.md` and `docs/postponed-backlog.md` as product-decision context before changing domain behavior.
- Historical plans in `.plans/` explain intent, especially `.plans/original-plan.md`. Use them for background, while treating the docs backlog and current code as the stronger source of truth.
- The original direction still matters: senior .NET practices, DDD-lite where useful, simple structure first, code-first EF Core, no repository pattern by default, and monthly finance behavior based on installments over time.

## ASP.NET Core Defaults

- Use modern ASP.NET Core patterns for .NET 10: `WebApplicationBuilder`, `WebApplication`, built-in dependency injection, options validation, hosted framework services, and the existing middleware pipeline style.
- Follow the existing controller-based API style. Do not convert controllers to Minimal APIs unless the user explicitly asks for that migration.
- Prefer built-in ASP.NET Core features before adding third-party infrastructure.
- Do not introduce preview-only APIs unless the user explicitly asks for preview work.
- Avoid dependency/version churn unless the task is about upgrades. If touching Microsoft or EF Core package versions, align the decision with the target framework deliberately.

## Architecture

- Keep controllers thin. Controllers should bind HTTP input, handle null request bodies, call a use case, and map `Result`/`Result<T>` through `ApiControllerBase`.
- Put application behavior in feature folders under `src/api/Features/<Domain>/<UseCase>/`.
- Name use case classes with the `UseCase` suffix so `AddApplicationServices` can register them automatically.
- Keep request validators separate via `IValidator<T>`. Validators should collect all validation errors that can be evaluated safely.
- Use `Result` and `Result<T>` for expected application failures. Do not use exceptions for validation, not found, or unauthorized outcomes.
- Keep response mapping explicit with mapper extensions/classes in the relevant feature's `Shared` folder when that pattern already exists.

## Domain Decisions

- Expenses model purchases; installments model the monthly occurrence used by monthly views.
- Monthly expense queries should be installment-centric, not expense-centric.
- Cash expenses still have one installment.
- Expense is the aggregate root for its installments and shares.
- Expense update, individual installment update, and individual installment delete are intentionally postponed. The current correction workflow is delete and recreate the expense.
- Explicit installment creation is supported for cases where statement values must match exact cents.
- Card association on expenses is optional and represented by `CardId`.
- Expense shares belong to expenses, use explicit share installments, and can become orphaned when a person is deleted.
- Use `DateOnly` for civil/business dates and `DateTimeOffset` for token/session instants unless there is a clear reason to do otherwise.
- Use the `Money` value object for monetary values and preserve the two-decimal-place invariant.

## Data Access

- Use `FenixContext` directly from use cases. Do not add a repository layer unless the user asks or a concrete duplication/problem justifies it.
- Use `IEntityTypeConfiguration<T>` mapping classes under `src/api/Data/Mappings`.
- Keep EF queries scoped to `ICurrentUser.UserId` for user-owned resources.
- Use `AsNoTracking()` for read-only queries.
- Pass `CancellationToken` through async EF and use case calls.
- Keep PostgreSQL/code-first migrations in `src/api/Data/Migrations`.

## Authentication And Authorization

- Authentication is configured through `AddFenixAuth` and applied through `UseFenixAuth`.
- The API currently uses JWT bearer authentication, with token resolution from the configured transport and an auth cookie named by `AuthCookieNames`.
- Authorization has a fallback policy requiring authenticated users. Use `[AllowAnonymous]` only for intentional public endpoints such as login/logout.
- Application logic should depend on `ICurrentUser`, not directly on `HttpContext.User`.
- Ownership checks belong in use cases and queries, not only in controllers.
- Do not log tokens, passwords, signing keys, cookies, or sensitive user data.

## Error Handling

- Current HTTP error responses use `{ errors = result.Errors }` through `ApiControllerBase`.
- Keep `AppError` codes stable, specific, and namespaced, for example `expense.card_id.invalid`.
- `Result` failures must contain at least one error and cannot mix error types.
- `ProblemDetails` and a final global exception/error contract are postponed decisions. Do not force that migration incidentally.

## Observability

- Preserve the existing observability foundation: W3C trace IDs, request logging middleware, correlation IDs, HTTP metrics, database command metrics, Prometheus `/metrics`, and OTLP tracing when configured.
- Keep `/metrics` excluded from normal request telemetry where the existing code does so.
- When adding new expected error paths, ensure `ApiControllerBase` can track the application error metadata.
- Prefer structured logs with useful identifiers over free-form diagnostic text.

## Testing And Verification

- There is no dedicated test project in the current tree. If a task changes behavior, consider whether adding focused tests is part of the task or call out the current test gap.
- At minimum, run `dotnet build src/api/api.csproj` after code changes when feasible.
- For API behavior changes, prefer future integration tests around HTTP behavior, auth/authorization, validation response shape, and EF persistence.
- Local infrastructure is started with `docker compose up -d`; the API is run with `dotnet run --project src/api --launch-profile http`.

## Backlog Awareness

- The active next product item is `Add Monthly Summary Read Model` in `docs/backlog.md`.
- Postponed items include expense update, installment update, installment delete, identifier strategy review, and final HTTP error response modeling.
- Do not implement postponed items while working on adjacent features unless the user explicitly moves them back into scope.
