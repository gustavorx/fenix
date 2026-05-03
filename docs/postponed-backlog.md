# Postponed Backlog

Items removed from the active backlog but intentionally kept for later reconsideration.

## Expense Update

Context

The API already supports creating and reading expenses, but it still lacks an endpoint and application flow to update an existing expense.

Decision

Deferred. The current product direction is to avoid expense update for now. If a user needs a different installment shape or corrected values, the supported workflow is deleting the expense and creating a new one.

## Installment Update

Context

Installments are already exposed in responses, but the API still lacks an endpoint and application flow to update an installment directly.

Decision

Deferred. The current product direction is to avoid installment update for now and keep schedule corrections in the expense delete-and-recreate workflow.

## Installment Delete

Context

The API still lacks an endpoint and application flow to delete an installment directly.

Decision

Deferred. The current product direction is to avoid individual installment delete for now and keep schedule corrections in the expense delete-and-recreate workflow.

## Review Identifier Strategy

Context

The domain currently uses primitive `Guid` identifiers directly across entities and persistence mappings, without a broader decision on whether IDs should remain `Guid`, move to sequential identifiers, or adopt typed value objects such as `UserId` or `ExpenseId`.

Decision

Deferred. Revisit identifier strategy once the domain surface is more stable and there is enough evidence to evaluate trade-offs across domain clarity, refactor cost, API contracts, EF mappings, and database performance characteristics such as index fragmentation and lookup behavior.

## HTTP Error Response

Context

The HTTP layer still builds some error payloads inline in controllers, but the API does not yet have its final HTTP error contract or the planned global exception middleware.

Decision

Deferred. Revisit HTTP error response modeling after introducing global exception handling and more endpoints, so the project can define a single error contract with better evidence about validation, not found, conflict, and unexpected failure scenarios.

## Move Metrics Export To OpenTelemetry Collector

Context

The API currently exposes `/metrics` directly through the Prometheus ASP.NET Core exporter, and Prometheus scrapes the API endpoint. This is practical for local development, but it keeps Prometheus-specific metric export inside the application and leaves `/metrics` as an operational endpoint on the API surface.

Decision

Deferred. Keep the current `/metrics` endpoint for now. Revisit metrics export later and move metrics to OTLP through the OpenTelemetry Collector, so Prometheus and Grafana consume metrics through the observability infrastructure instead of scraping the API directly.

## Standardize Application Error Codes And Messages

Context

Validation errors currently use `AppError.Validation(code, message)` across controllers, validators, and use cases, but the `code` and `message` conventions are not fully consistent.

Motivation

Inconsistent error codes and messages make frontend handling, observability, documentation, and future localization harder. As the API grows, these inconsistencies become more expensive to fix.

Next step

Review all `AppError.Validation` usages and define a consistent convention for error code namespaces, field naming, message tone, and whether messages should be user-facing or developer-facing. Apply the convention incrementally and consider centralizing repeated error factories where it improves consistency.

Decision

Deferred. Revisit once the API surface is broader and there is enough evidence to standardize error conventions without slowing active feature work.

## Run Security Threat Model Review

Context

The API now has authentication, ownership checks, expense sharing, people, cards, and financial read models, but it has not had a dedicated security threat model pass.

Motivation

Financial data and user-scoped resources need explicit review for authorization gaps, cross-user access, unsafe state transitions, sensitive data exposure, and abuse cases around shared expenses and orphaned shares.

Next step

Run a focused threat model review using the `security-threat-model` skill. Capture assets, trust boundaries, entry points, attacker goals, likely abuse cases, and recommended mitigations, then turn any concrete issues into implementation backlog items.

Decision

Deferred. Keep the review available for a later dedicated security pass instead of mixing it into the active product backlog.
