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
