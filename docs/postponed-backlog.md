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
