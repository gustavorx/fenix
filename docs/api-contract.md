# API Contract For Frontend Work

This document is the human-readable contract for the current Fenix API surface. The generated OpenAPI document is the technical companion, but this guide records the product workflows, browser auth behavior, response conventions, and frontend-facing rules.

## OpenAPI

The API exposes OpenAPI only in Development:

- `GET /openapi/v1.json`
- No Swagger UI is configured.
- Verified on 2026-05-04: the generated document returned `200`, exposed 19 paths, and included request/response schemas such as `LoginRequest`, `MonthlySummaryResponse`, `ExpenseResponse`, and `ApiErrorResponse`.

Run locally:

```powershell
docker compose up -d
dotnet run --project src/api --launch-profile http
```

The HTTP launch profile listens on `http://localhost:5207`.

## Request Conventions

- API routes are prefixed with `/api`.
- JSON request and response bodies use camel case in HTTP payloads, even though C# DTO properties are PascalCase.
- `DateOnly` fields are sent as ISO dates: `YYYY-MM-DD`.
- Money fields are JSON numbers and must preserve at most two decimal places.
- Resource identifiers are GUID strings.
- Monthly query endpoints use `month` and `year` query parameters.
- Enum values are numeric:
  - `paymentType`: `1` = Cash, `2` = Installment.
  - `installmentCreateMode`: `1` = Generated, `2` = Explicit.

## Authentication

The API uses JWT bearer authentication with cookie transport for browser clients.

- `POST /api/auth/login` validates credentials, returns a login payload, and sets the auth cookie.
- `POST /api/auth/logout` clears the auth cookie.
- `GET /api/auth/me` returns the current authenticated user.
- The auth cookie name is `fenix_auth_token`.
- The cookie is `HttpOnly`, `SameSite=Lax`, `Path=/`, and is secure outside Development.
- Browser frontend requests must include credentials:

```ts
fetch("/api/monthly-summary?month=4&year=2026", {
  credentials: "include",
});
```

The login response currently includes the token in the JSON body as well as the cookie. Browser clients should use the cookie-backed session behavior and should not store the token manually.

All endpoints require authentication by fallback authorization policy except:

- `POST /api/auth/login`
- `POST /api/auth/logout`

## Error Responses

Expected application failures use one shared shape:

```json
{
  "errors": [
    {
      "code": "income.amount.invalid",
      "message": "Amount must be greater than zero.",
      "type": 0
    }
  ]
}
```

Frontend behavior should depend on `code` and HTTP status, not on parsing message text.

Common statuses:

- `400 Bad Request`: validation failure.
- `401 Unauthorized`: missing, expired, or invalid authentication.
- `404 Not Found`: resource does not exist or is not owned by the current user.
- `204 No Content`: successful delete/logout with no body.

## Auth Workflow

### `POST /api/auth/login`

Request:

```json
{
  "email": "user@example.com",
  "password": "secret"
}
```

Response `200`:

```json
{
  "token": "jwt",
  "expiresAt": "2026-05-04T04:00:00+00:00",
  "user": {
    "id": "00000000-0000-0000-0000-000000000000",
    "name": "User",
    "email": "user@example.com"
  }
}
```

Errors: `400`, `401`.

### `POST /api/auth/logout`

Response: `204`.

### `GET /api/auth/me`

Response `200`:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "User",
  "email": "user@example.com"
}
```

Errors: `401`.

## Monthly Dashboard Workflow

### `GET /api/monthly-summary?month=4&year=2026`

Response `200`:

```json
{
  "month": 4,
  "year": 2026,
  "totals": {
    "totalIncomes": 9904.93,
    "totalSharedReceivables": 800.00,
    "totalGrossExpenses": 10577.30,
    "totalNetExpenses": 9777.30,
    "myFinalBalance": 127.63
  }
}
```

This is the dashboard header read model. The frontend should use it instead of recalculating top-level totals from separate lists.

Rules:

- `totalIncomes`: incomes received in the month.
- `totalSharedReceivables`: share installments due in the month, regardless of payment status.
- `totalGrossExpenses`: expense installments due in the month before subtracting shares.
- `totalNetExpenses`: `totalGrossExpenses - totalSharedReceivables`.
- `myFinalBalance`: `totalIncomes - totalNetExpenses`.

Errors: `400`, `401`.

## Incomes Workflow

Endpoints:

- `GET /api/incomes`
- `GET /api/incomes/{id}`
- `GET /api/incomes/monthly?month=4&year=2026`
- `POST /api/incomes`
- `PATCH /api/incomes/{id}`
- `DELETE /api/incomes/{id}`

Create request:

```json
{
  "description": "Salary",
  "amount": 9904.93,
  "receivedDate": "2026-04-05"
}
```

Update request accepts any subset of:

```json
{
  "description": "Salary",
  "amount": 9904.93,
  "receivedDate": "2026-04-05"
}
```

Income response:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "description": "Salary",
  "amount": 9904.93,
  "receivedDate": "2026-04-05"
}
```

Monthly response:

```json
{
  "month": 4,
  "year": 2026,
  "totalAmount": 9904.93,
  "incomes": []
}
```

Rules:

- Description is required on create.
- Amount must be greater than zero and use at most two decimal places.
- Received date is required on create.
- Update requires at least one field.

## Expenses Workflow

Endpoints:

- `GET /api/expenses`
- `GET /api/expenses/{id}`
- `GET /api/expenses/monthly?month=4&year=2026`
- `POST /api/expenses`
- `DELETE /api/expenses/{id}`

Expense update, individual installment update, and individual installment delete are intentionally out of scope. Correction is delete and recreate.

Generated installment create request:

```json
{
  "description": "Notebook",
  "totalAmount": 3000.00,
  "purchaseDate": "2026-04-10",
  "paymentType": 2,
  "totalInstallments": 3,
  "firstDueDate": "2026-05-10",
  "cardId": "00000000-0000-0000-0000-000000000000",
  "installmentCreateMode": 1
}
```

Explicit installment create request:

```json
{
  "description": "Notebook",
  "purchaseDate": "2026-04-10",
  "paymentType": 2,
  "cardId": null,
  "installmentCreateMode": 2,
  "installments": [
    { "amount": 1000.00, "dueDate": "2026-05-10" },
    { "amount": 1000.00, "dueDate": "2026-06-10" },
    { "amount": 1000.00, "dueDate": "2026-07-10" }
  ]
}
```

Create may also include `shares` when the share workflow is known at creation time:

```json
{
  "shares": [
    {
      "personId": "00000000-0000-0000-0000-000000000000",
      "installments": [
        { "amount": 250.00, "dueDate": "2026-05-10" }
      ]
    }
  ]
}
```

Expense response:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "description": "Notebook",
  "totalAmount": 3000.00,
  "purchaseDate": "2026-04-10",
  "paymentType": 2,
  "totalInstallments": 3,
  "cardId": null,
  "installments": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "number": 1,
      "amount": 1000.00,
      "dueDate": "2026-05-10",
      "paid": false
    }
  ]
}
```

Expense detail response adds `shares`.

Monthly expenses response:

```json
{
  "month": 4,
  "year": 2026,
  "totalAmount": 10577.30,
  "installments": [
    {
      "installmentId": "00000000-0000-0000-0000-000000000000",
      "expenseId": "00000000-0000-0000-0000-000000000000",
      "cardId": null,
      "description": "Notebook",
      "paymentType": 2,
      "totalAmount": 3000.00,
      "totalInstallments": 3,
      "installmentNumber": 1,
      "installmentAmount": 1000.00,
      "purchaseDate": "2026-04-10",
      "dueDate": "2026-05-10",
      "paid": false,
      "hasShares": true
    }
  ]
}
```

Rules:

- Monthly expense views are installment-centric.
- Cash expenses still create one installment.
- In generated mode, `totalAmount` is required and `installments` must not be sent.
- In explicit mode, `installments` are required, and `totalAmount`, `totalInstallments`, and `firstDueDate` must not be sent.
- `cardId` is optional, but if provided it must belong to the current user.
- Shares cannot exceed the expense total.

## Cards Workflow

Endpoints:

- `GET /api/cards`
- `GET /api/cards/{id}`
- `POST /api/cards`
- `PATCH /api/cards/{id}`
- `DELETE /api/cards/{id}`

Create or update request:

```json
{
  "name": "Main credit card",
  "limit": 5000.00,
  "closingDay": 20
}
```

Response:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Main credit card",
  "limit": 5000.00,
  "closingDay": 20
}
```

Rules:

- Name is required on create and limited to 100 characters.
- Limit is optional, but when present must be greater than zero and use at most two decimal places.
- Closing day is optional, but when present must be between 1 and 31.
- Update requires at least one field.

## People Workflow

Endpoints:

- `GET /api/people`
- `GET /api/people/{id}`
- `POST /api/people`
- `PATCH /api/people/{id}`
- `DELETE /api/people/{id}`

Create or update request:

```json
{
  "name": "Alex"
}
```

Response:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "name": "Alex"
}
```

Rules:

- Name is required and limited to 100 characters.
- Update requires at least one field with a value.
- Deleting a person can orphan existing shares; expense share responses may have `personId` and `personName` as null.

## Expense Shares Workflow

Endpoints:

- `POST /api/expenses/{expenseId}/shares`
- `PATCH /api/expenses/{expenseId}/shares/{shareId}`
- `DELETE /api/expenses/{expenseId}/shares/{shareId}`

Create request:

```json
{
  "personId": "00000000-0000-0000-0000-000000000000",
  "installments": [
    { "amount": 250.00, "dueDate": "2026-05-10" }
  ]
}
```

Update request:

```json
{
  "personId": "00000000-0000-0000-0000-000000000000"
}
```

Response:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "personId": "00000000-0000-0000-0000-000000000000",
  "personName": "Alex",
  "amount": 250.00,
  "paidAmount": 0.00,
  "outstandingAmount": 250.00,
  "isFullyPaid": false,
  "installments": []
}
```

Rules:

- `personId` is required and must belong to the current user.
- At least one share installment is required on create.
- Share installment amounts must be greater than zero and use at most two decimal places.
- The total shared amount for an expense cannot exceed the expense total.

## Expense Share Installments Workflow

Endpoints:

- `GET /api/expense-share-installments/monthly?month=4&year=2026`
- `POST /api/expenses/{expenseId}/shares/{shareId}/installments`
- `PATCH /api/expenses/{expenseId}/shares/{shareId}/installments/{installmentId}`
- `DELETE /api/expenses/{expenseId}/shares/{shareId}/installments/{installmentId}`

Create request:

```json
{
  "amount": 250.00,
  "dueDate": "2026-05-10"
}
```

Update request accepts any subset of:

```json
{
  "amount": 250.00,
  "dueDate": "2026-05-10",
  "paidDate": "2026-05-15"
}
```

Send `"paidDate": null` to explicitly clear payment. Omit `paidDate` to leave payment status unchanged.

Monthly response:

```json
{
  "month": 4,
  "year": 2026,
  "totalAmount": 800.00,
  "items": [
    {
      "shareInstallmentId": "00000000-0000-0000-0000-000000000000",
      "shareId": "00000000-0000-0000-0000-000000000000",
      "expenseId": "00000000-0000-0000-0000-000000000000",
      "expenseDescription": "Notebook",
      "personId": "00000000-0000-0000-0000-000000000000",
      "personName": "Alex",
      "amount": 250.00,
      "dueDate": "2026-05-10",
      "paidDate": null,
      "isPaid": false
    }
  ]
}
```

Rules:

- Create requires amount and due date.
- Amount must be greater than zero and use at most two decimal places.
- Share installment mutations return the parent share response.
- Deleting the last share installment is not allowed by current business rules.
