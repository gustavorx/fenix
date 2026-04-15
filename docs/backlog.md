# Backlog

## Summary

- [x] ~~[1. Review Date And Timezone Modeling](#1-review-date-and-timezone-modeling)~~
- [x] ~~[2. Replace Payment Type Strings With Enums](#2-replace-payment-type-strings-with-enums)~~
- [x] ~~[3. Extract Validation From Use Cases](#3-extract-validation-from-use-cases)~~
- [x] ~~[4. Refactor To Rich Domain Model](#4-refactor-to-rich-domain-model)~~
- [x] ~~[5. Define Expense And Installment Mutation Rules](#5-define-expense-and-installment-mutation-rules)~~
- [x] ~~[6. Decide Expense And Installment Mutation Scenarios](#6-decide-expense-and-installment-mutation-scenarios)~~
- [x] ~~[7. Auth And Authorization Phase 1](#7-auth-and-authorization-phase-1)~~
- [X] [8. Add Observability Foundation](#8-add-observability-foundation)
- [X] [9. Add Expense Delete Endpoint](#9-add-expense-delete-endpoint)
- [X] [10. Add Income Update Endpoint](#10-add-income-update-endpoint)
- [X] [11. Add Income Delete Endpoint](#11-add-income-delete-endpoint)
- [ ] [12. Add Cards And Optional Expense Card Association](#12-add-cards-and-optional-expense-card-association)
- [ ] [13. Add Explicit Installment Create Mode](#13-add-explicit-installment-create-mode)
- [ ] [14. Auth And Authorization Phase 2](#14-auth-and-authorization-phase-2)

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

Decision

Adopted rules:

- `Expense` remains the aggregate root and owns installment lifecycle decisions.
- Expense creation will support both generated installments and explicit installments supplied by the client.
- For installment expenses, the safest source of truth is the persisted installment list, with `TotalAmount` derived from the sum of installments.
- Expense update is out of scope for now; if the user needs a different shape, the supported workflow is delete and recreate.
- Expense delete means physical removal of the whole aggregate, including expenses with paid installments.
- Individual installment update and delete are out of scope for now.

## 6. Decide Expense And Installment Mutation Scenarios

Context

There are still open business decisions that directly affect the design of update and delete endpoints for expenses and installments.

Motivation

If these scenarios remain undefined, the implementation may choose behavior implicitly and force later changes in contracts, persistence logic, and tests.

Decision

The expected behavior for the open scenarios is now fixed:

- Updating an expense does not regenerate installments because expense update is not supported for now.
- The parent expense does not adjust `TotalAmount` or `InstallmentsQuantity` after individual installment delete because individual installment delete is not allowed.
- An installment cannot be deleted individually in this phase.
- Paid installments are not recalculated or replaced; corrections happen by deleting and recreating the full expense.

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

- Phase 1: Add structured HTTP request logging, correlation identifiers, trace identifiers, request duration, status code logging, known application error metadata, and unexpected exception logging. Database metrics and dashboards are explicitly out of scope for this phase.
- Phase 2: Add HTTP metrics for request count, request duration, status code distribution, and error rates, preferably with OpenTelemetry-compatible instrumentation.
- Phase 3: Add database instrumentation for command count, command duration, and command failures, preferably through OpenTelemetry instrumentation or an EF Core command interceptor rather than use-case-level timers.
- Phase 4: Add tracing for HTTP requests and database operations, preferably with OpenTelemetry-compatible instrumentation and W3C trace context propagation.
- Phase 5: Add local dashboards and telemetry collection infrastructure with OpenTelemetry Collector, Prometheus, Tempo, and Grafana. Keep metrics on Prometheus scrape from `/metrics` and export traces via OTLP to the collector.
- Phase 6: Define a repeatable load-test baseline and use observability data to measure throughput, latency, error rate, and saturation under load.

## 9. Add Expense Delete Endpoint

Context

The API still lacks an endpoint and application flow to delete an expense.

Motivation

Deleting an expense is core functionality, but its behavior must remain aligned with installment ownership, cascade rules, and authorization.

Next step

Implement the use case and controller endpoint for deleting expenses with physical removal of the full aggregate, even when some installments were already marked as paid.

## 10. Add Income Update Endpoint

Context

The API already supports creating and reading incomes, but it still lacks an endpoint and application flow to update an existing income.

Motivation

Income update is part of the expected CRUD surface and is structurally simpler than expense and installment mutation, but it should still follow the same validation and user-scoping conventions.

Next step

Implement the request model, validator, use case, and controller endpoint for updating incomes after the shared application-layer foundations are in place.

## 11. Add Income Delete Endpoint

Context

The API still lacks an endpoint and application flow to delete an income.

Motivation

Income delete is part of the core API surface and should be added consistently with validation, authorization, and HTTP response conventions.

Next step

Implement the use case and controller endpoint for deleting incomes after the shared application-layer foundations are in place.

## 12. Add Cards And Optional Expense Card Association

Context

The domain and persistence model already include `Card` and an optional `Expense.CardId`, but the API still does not expose card CRUD or allow clients to associate a card explicitly when creating or reading expenses.

Motivation

Card becomes part of the expense identity for many real flows, especially installment purchases. If the API does not expose cards consistently, the client cannot model statement-based spending accurately and the monthly expense view stays ambiguous about whether a purchase belongs to cash flow or a specific card.

Decision

Adopt these rules for this phase:

- Card association on expense is optional. An expense may exist without any linked card.
- The source of truth for card association is the expense itself through `CardId`.
- Monthly expense response should remain installment-centric, with a single `installments[]` list. Installments whose parent expense has a linked card should include an optional `cardId`, instead of returning a separate top-level card grouping.
- Expense read responses should also expose the same optional `cardId` so the contract stays consistent between `GET /expenses`, `GET /expenses/{id}`, and `GET /expenses/monthly`.
- Expense responses should expose only the linked `cardId`. Card details continue to be resolved through card endpoints when needed.
- Expense update remains out of scope for now. Changing card association on an existing expense continues to follow the current delete-and-recreate workflow unless a dedicated mutation is added later.

Suggested phases

- Phase 1: Define card contract and implement card CRUD endpoints with user scoping, validation, and ownership checks. Done.
- Phase 2: Extend expense create request with optional `cardId` and validate that the referenced card belongs to the current user.
- Phase 3: Expose optional `cardId` in expense responses and monthly installment responses.
- Phase 4: Revisit whether card-specific monthly aggregations deserve a separate endpoint such as a future statement-oriented query, but keep that out of the current expense-month contract.

## 13. Add Explicit Installment Create Mode

Context

The API already generates installments automatically, but some card statements distribute cents across installments in a way that may not match the server split logic.

Motivation

If the user cannot provide the exact installment values seen in the card statement, balances may diverge by cents and the application stops being a faithful mirror of the user's financial reality.

Next step

Extend expense creation so the client can choose between:

- generated schedule mode, where the server derives installments from aggregate inputs;
- explicit schedule mode, where the client sends `installments[]` with exact amounts and due dates and the server derives `TotalAmount` from that list.

## 14. Auth And Authorization Phase 2

Context

The core API can evolve for a while with a fixed user behind `ICurrentUser`, but the product will eventually need real authentication and authorization flows for multiple users.

Motivation

Identity flows such as user creation, email validation, token issuance, and session management are important, but they are not required to keep evolving the core business API if phase 1 already isolates user context correctly.

Next step

Replace the fixed current-user implementation with real authentication and authorization flows, such as JWT in cookies or another session mechanism, once the core API surface is stable.
