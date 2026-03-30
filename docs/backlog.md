# Backlog

## Summary

- [x] ~~[1. Review Date And Timezone Modeling](#1-review-date-and-timezone-modeling)~~
- [x] ~~[2. Replace Payment Type Strings With Enums](#2-replace-payment-type-strings-with-enums)~~
- [x] ~~[3. Extract Validation From Use Cases](#3-extract-validation-from-use-cases)~~
- [ ] [4. Refactor To Rich Domain Model](#4-refactor-to-rich-domain-model)
- [ ] [5. Define Expense And Installment Mutation Rules](#5-define-expense-and-installment-mutation-rules)
- [ ] [6. Decide Expense And Installment Mutation Scenarios](#6-decide-expense-and-installment-mutation-scenarios)
- [ ] [7. Auth And Authorization Phase 1](#7-auth-and-authorization-phase-1)
- [ ] [8. Add Observability Foundation](#8-add-observability-foundation)
- [ ] [9. HTTP Error Response](#9-http-error-response)
- [ ] [10. Add Expense Update Endpoint](#10-add-expense-update-endpoint)
- [ ] [11. Add Expense Delete Endpoint](#11-add-expense-delete-endpoint)
- [ ] [12. Add Installment Update Endpoint](#12-add-installment-update-endpoint)
- [ ] [13. Add Installment Delete Endpoint](#13-add-installment-delete-endpoint)
- [ ] [14. Add Income Update Endpoint](#14-add-income-update-endpoint)
- [ ] [15. Add Income Delete Endpoint](#15-add-income-delete-endpoint)
- [ ] [16. Auth And Authorization Phase 2](#16-auth-and-authorization-phase-2)

## 1. Review Date And Timezone Modeling

Context

The current date handling still relies on `DateTime` plus `DateRules.Normalize`, which mixes UTC normalization concerns with default-value behavior.

Motivation

Not every domain date represents an instant in time. Some values may be business dates instead, and treating everything as UTC can hide semantic mismatches.

Next step

Review which fields represent UTC instants and which represent civil dates, then revisit `DateRules` and evaluate whether `DateTimeOffset`, `DateOnly`, or explicit UTC conversion at the application boundary would model the domain more clearly.

## 2. Replace Payment Type Strings With Enums

Context

Expense payment type still depends on string normalization rules such as `NormalizePaymentType`.

Motivation

This increases validation and mapping complexity and keeps invalid values possible longer than necessary.

Next step

Replace payment type strings with enums and remove normalization helpers that become unnecessary after the type change.

## 3. Extract Validation From Use Cases

Context

Validation rules for create operations are still implemented directly inside use cases.

Motivation

The current approach is explicit, but it tends to pollute use cases and mixes orchestration with validation rules.

Next step

Introduce dedicated validator classes per request or a shared `IValidator<T>` abstraction that returns `IReadOnlyList<AppError>`, keeping the application layer based on `Result<T>` instead of exceptions.

## 4. Refactor To Rich Domain Model

Context

Business rules are still concentrated in use cases and helper classes, which keeps the domain entities mostly anemic.

Motivation

If the project continues adding mutations in the current style, domain behavior will become more scattered across features and harder to evolve consistently. Moving to a richer domain model now is still feasible because the API surface is relatively small.

Next step

Refactor the domain so that entities and aggregates own their invariants and state transitions. In particular, move expense and installment behavior closer to the domain model, keep use cases focused on orchestration and authorization, and reduce dependence on feature-local rule helpers for business behavior.

## 5. Define Expense And Installment Mutation Rules

Context

The current model already treats `Expense` as the aggregate root that owns installments, but the API still does not define how update and delete operations should behave for `expenses` and `installments`.

Motivation

Without explicit mutation rules, new endpoints may encode inconsistent behavior and force refactors across use cases, persistence, and response contracts.

Next step

Define the application rules before implementing new endpoints. At minimum, decide whether updating an expense can regenerate installments, whether an installment can be deleted independently, how paid installments constrain edits, and whether delete means physical removal or a softer business action.

## 6. Decide Expense And Installment Mutation Scenarios

Context

There are still open business decisions that directly affect the design of update and delete endpoints for expenses and installments.

Motivation

If these scenarios remain undefined, the implementation may choose behavior implicitly and force later changes in contracts, persistence logic, and tests.

Next step

Define the expected behavior for the following scenarios:

- Can updating an expense regenerate its installments?
- If an installment is deleted, should the parent expense adjust `TotalAmount` and `InstallmentsQuantity`?
- Can an installment be deleted individually, or should it only be canceled, skipped, or marked in another way?
- Can paid installments be recalculated or replaced?

## 7. Auth And Authorization Phase 1

Context

The application still uses a hard-coded default user in the application layer and does not yet scope reads or writes through an authenticated user context.

Motivation

New update and delete endpoints will increase the cost of adding user scoping later if ownership and authorization rules remain implicit. At the same time, the full identity flows for user registration, email validation, token issuance, and session management do not need to block the core product API.

Next step

Introduce an application-facing current-user abstraction, for example `ICurrentUser`, and enforce ownership scoping in use cases and queries while still allowing a fixed development user behind that abstraction.

Keep authorization rules explicit in the application layer so the transport-level authentication mechanism can change later without rewriting the use cases.

## 8. Add Observability Foundation

Context

The API still lacks basic observability for HTTP requests, database operations, failures, and resource consumption, which makes performance analysis and capacity planning mostly guesswork.

Motivation

Future load tests will be much more useful if the application already exposes request latency, database latency, error rates, throughput, and enough tracing to explain where time is being spent.

Next step

Introduce an observability foundation before running meaningful load tests. At minimum, capture structured logs, request duration, database query duration, error counts, and correlation or trace identifiers. Prefer a design that can evolve into metrics dashboards and distributed tracing later, such as OpenTelemetry-based instrumentation.

Track, at minimum:

- HTTP request count, duration, and status code distribution
- Error response count by endpoint and error type
- Database command count and duration
- Trace or correlation identifiers across request and persistence layers
- Basic process telemetry such as CPU and memory where feasible in the hosting environment

Suggested phases:

- Phase 1: Add structured logging, correlation identifiers, and basic request logging for the HTTP layer.
- Phase 2: Add metrics for HTTP requests, status codes, error rates, and database command duration.
- Phase 3: Add tracing for HTTP requests and database operations, preferably with OpenTelemetry-compatible instrumentation.
- Phase 4: Add local dashboards and telemetry collection infrastructure, such as an OpenTelemetry Collector plus a metrics and tracing backend.
- Phase 5: Define a repeatable load-test baseline and use observability data to measure throughput, latency, error rate, and saturation under load.

## 9. HTTP Error Response

Context

The HTTP layer still builds error payloads inline in controllers for some cases such as `request == null`.

Motivation

This works for now, but it leaks response-shaping concerns into controllers and makes the HTTP boundary less consistent than it could be.

Next step

Create a dedicated HTTP error response model, for example `ApiErrorResponse`, and centralize error payload construction in the controller base or a mapper.

## 10. Add Expense Update Endpoint

Context

The API already supports creating and reading expenses, but it still lacks an endpoint and application flow to update an existing expense.

Motivation

Expense updates are part of the core product workflow, but they depend on the mutation rules, payment type model, date model, and user scoping being clear enough first.

Next step

Implement the request model, validator, use case, and controller endpoint for updating expenses after the mutation rules and application-level prerequisites are settled.

## 11. Add Expense Delete Endpoint

Context

The API still lacks an endpoint and application flow to delete an expense.

Motivation

Deleting an expense is core functionality, but its behavior must remain aligned with installment ownership, cascade rules, and authorization.

Next step

Implement the use case and controller endpoint for deleting expenses after the expense and installment mutation rules are defined.

## 12. Add Installment Update Endpoint

Context

Installments are already exposed in responses, but the API still lacks an endpoint and application flow to update an installment directly.

Motivation

This feature is likely to be useful in the product, especially for due date and payment-state management, but it depends on clear ownership and recalculation rules.

Next step

Implement the request model, validator, use case, and controller endpoint for updating installments after the mutation scenarios are defined.

## 13. Add Installment Delete Endpoint

Context

The API still lacks an endpoint and application flow to delete an installment directly.

Motivation

This operation is especially sensitive because installments belong to an expense aggregate, so delete semantics need to be explicit before coding.

Next step

Implement the use case and controller endpoint for deleting installments only after defining whether individual deletion is allowed and how the parent expense should react.

## 14. Add Income Update Endpoint

Context

The API already supports creating and reading incomes, but it still lacks an endpoint and application flow to update an existing income.

Motivation

Income update is part of the expected CRUD surface and is structurally simpler than expense and installment mutation, but it should still follow the same validation and user-scoping conventions.

Next step

Implement the request model, validator, use case, and controller endpoint for updating incomes after the shared application-layer foundations are in place.

## 15. Add Income Delete Endpoint

Context

The API still lacks an endpoint and application flow to delete an income.

Motivation

Income delete is part of the core API surface and should be added consistently with validation, authorization, and HTTP response conventions.

Next step

Implement the use case and controller endpoint for deleting incomes after the shared application-layer foundations are in place.

## 16. Auth And Authorization Phase 2

Context

The core API can evolve for a while with a fixed user behind `ICurrentUser`, but the product will eventually need real authentication and authorization flows for multiple users.

Motivation

Identity flows such as user creation, email validation, token issuance, and session management are important, but they are not required to keep evolving the core business API if phase 1 already isolates user context correctly.

Next step

Replace the fixed current-user implementation with real authentication and authorization flows, such as JWT in cookies or another session mechanism, once the core API surface is stable.
