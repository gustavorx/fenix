# Backlog

## HTTP Error Response

Context

The HTTP layer still builds error payloads inline in controllers for some cases such as `request == null`.

Motivation

This works for now, but it leaks response-shaping concerns into controllers and makes the HTTP boundary less consistent than it could be.

Next step

Create a dedicated HTTP error response model, for example `ApiErrorResponse`, and centralize error payload construction in the controller base or a mapper.

## Extract Validation From Use Cases

Context

Validation rules for create operations are still implemented directly inside use cases.

Motivation

The current approach is explicit, but it tends to pollute use cases and mixes orchestration with validation rules.

Next step

Introduce dedicated validator classes per request or a shared `IValidator<T>` abstraction that returns `IReadOnlyList<AppError>`, keeping the application layer based on `Result<T>` instead of exceptions.

## Replace Payment Type Strings With Enums

Context

Expense payment type still depends on string normalization rules such as `NormalizePaymentType`.

Motivation

This increases validation and mapping complexity and keeps invalid values possible longer than necessary.

Next step

Replace payment type strings with enums and remove normalization helpers that become unnecessary after the type change.

## Review Date And Timezone Modeling

Context

The current date handling still relies on `DateTime` plus `DateRules.Normalize`, which mixes UTC normalization concerns with default-value behavior.

Motivation

Not every domain date represents an instant in time. Some values may be business dates instead, and treating everything as UTC can hide semantic mismatches.

Next step

Review which fields represent UTC instants and which represent civil dates, then revisit `DateRules` and evaluate whether `DateTimeOffset`, `DateOnly`, or explicit UTC conversion at the application boundary would model the domain more clearly.
