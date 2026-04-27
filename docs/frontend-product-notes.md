# Frontend Product Notes

These notes capture the first frontend product shape from the masked spreadsheet reference:

- `docs/references/monthly-spreadsheet-masked.png`

The goal is not to recreate the spreadsheet cell-for-cell. The goal is to preserve the monthly finance workflow, the labels that already make sense to the user, and the information density needed for a dashboard-first frontend.

## Mental Model

The current workflow is a monthly money overview centered on answering one question quickly:

> After income, shared receivables, fixed costs, cards, debit/other expenses, and transfers to other people, how much is left?

The spreadsheet is organized as a set of monthly buckets:

- fixed expenses;
- credit card statement groups by card and due date;
- debit and other direct expenses;
- income summary;
- shared amounts owed by other people.

The first frontend version should feel like a finance workbench for scanning the month, not like a generic CRUD admin.

## Reference Layout

The screenshot has four main visual zones.

### Fixed Expenses

Label in spreadsheet: `GASTOS FIXOS`.

Columns:

- `O QUE`
- `VALOR`
- `OBS`

The section has a total row with:

- total amount;
- percentage of total expenses/income context.

Example rows:

- recurring fixed costs;
- observation values such as `Dia 01`, `Dia 03`, `Dia 10`.

Frontend implication:

- Fixed monthly expenses should be easy to scan as recurring obligations.
- The due day is important enough to show in the dashboard table/list.

### Credit Cards

Labels in spreadsheet:

- `NUBANK - VCTO. 01`
- `ITAÚ - VCTO. 09`

Columns:

- `O QUE`
- `VALOR`
- `OBS`

The section has a total row with:

- total card amount;
- percentage contribution.

Example rows:

- cash-like card purchase: `compra a vista A`;
- installments: `compra parcelada B`;
- fixed subscriptions: `assinatura F`, `assinatura G`.

Observation values include:

- installment progress, such as `4/10`, `6/6`, `8/12`, `12/12`;
- `FIXO` for recurring card items.

Frontend implication:

- Card expenses need to show card identity and due date.
- Installment progress is a first-class display value.
- Completed installment series should be visually distinguishable from still-active ones.
- Fixed subscriptions on cards should be visible, not hidden inside generic card spending.

### Debit And Other Expenses

Label in spreadsheet: `Débito / Outros`.

Columns:

- `O QUE`
- `VALOR`
- `OBS`

The section has a total row with:

- total direct expenses;
- percentage contribution.

Example rows:

- transfers/payments to people, such as `Pagar para o fulano` and `Pagar para o ciclano`;
- due day observations such as `Dia 01` and `Dia 09`.

Frontend implication:

- The dashboard should support non-card expenses separately from card expenses.
- Direct payments/transfers may need clear labels because they can look like expenses but are sometimes person-related.

### Income, Balance, And Shared Receivables

Top-right summary labels:

- `Entradas:`
- `Recebidos de alguém:`
- `Despesas`
- `Sobra:`

Summary values include:

- total income;
- amount received or receivable from someone;
- total expenses;
- remaining balance;
- percentages for income, expenses, and remaining balance.

Income rows shown:

- `Salário`
- `Dividendos`

Shared receivable rows shown:

- person name plus expense description, such as `Beltrano - compra parcelada B`;
- person name plus one-off purchase, such as `Deltano - compra a vista A`;
- amount;
- installment progress when applicable, such as `4/10`.

Frontend implication:

- The monthly summary should always be visible near the top of the dashboard.
- Shared receivables are part of the user's real monthly outcome, not a secondary-only feature.
- The user thinks in terms of "someone owes/paid me for this purchase", so the UI should include person name, source expense, amount, and installment progress.

## Key Monthly Totals

The first dashboard should show these totals as primary metrics:

- `Entradas`: total incomes for the selected month.
- `Recebidos de alguém`: shared receivables for the selected month.
- `Despesas`: total monthly expenses.
- `Sobra`: final remaining balance.

The API already maps these concepts in `GET /api/monthly-summary`:

- `totalIncomes`
- `totalSharedReceivables`
- `totalGrossExpenses`
- `totalNetExpenses`
- `myFinalBalance`

Product wording can stay close to the spreadsheet:

- `Entradas`
- `Recebidos de alguém`, meaning values to receive from someone in the selected month
- `Despesas`
- `Sobra`

`totalNetExpenses` is useful internally and may appear as a secondary metric, but the spreadsheet's visible language emphasizes gross `Despesas` and final `Sobra`.

## First Dashboard Requirements

The first frontend dashboard should make these visible without navigating away:

- selected month and year;
- summary metrics for `Entradas`, `Recebidos de alguém`, `Despesas`, and `Sobra`;
- income list or compact income breakdown;
- grouped monthly expenses;
- card identity for card expenses;
- card due date where available;
- debit/other expense grouping;
- fixed/recurring expense indicators;
- installment progress;
- shared receivable indicators;
- person-related receivables with amount and installment progress.

The dashboard should support scanning by group:

- fixed expenses;
- credit cards;
- debit/other;
- shared receivables;
- incomes.

## Secondary Screens

These can live outside the first dashboard:

- full income CRUD;
- full card CRUD;
- full people CRUD;
- full expense detail;
- full expense creation workflow;
- explicit installment creation details;
- share installment mutation details;
- historical management beyond the selected month.

The dashboard can link into these screens, but should not make the first version depend on exposing every mutation inline.

## Required Data Fields

### Expense Display

Required visible fields:

- description;
- amount for the month;
- payment type or group;
- card name when linked to a card;
- card due date or card closing/due context when available;
- due date or due day;
- installment number and total installments when installment-based;
- fixed/recurring marker when applicable;
- shared marker when the expense has shares.

API support:

- monthly expenses expose installment-centric rows;
- `cardId` is available, with card details resolved from card endpoints;
- `hasShares` is available for monthly expense rows.

### Income Display

Required visible fields:

- description;
- amount;
- received date.

### Shared Receivable Display

Required visible fields:

- person name;
- source expense description;
- amount;
- due date;
- paid status;
- paid date when available;
- installment progress when the receivable comes from an installment expense, if available in the read model.

Current API note:

- `GET /api/expense-share-installments/monthly` is the monthly read model for values to receive from someone.
- Monthly share installment rows include person, source expense, amount, due date, paid date, and paid status.
- The monthly share installment response does not currently expose installment progress like `4/10`; the frontend can show the receivable row without that exact suffix at first, or a future read-model change can add it directly.

## Product Decisions

- The frontend should be dashboard-first.
- The spreadsheet's section labels should influence UI language because they match the user's existing workflow.
- The first dashboard should initially stay very close to the spreadsheet structure, with visible grouped blocks instead of starting from a generic unified table.
- Monthly expense views should remain installment-centric.
- Credit cards should be grouped or filterable because card due dates are meaningful.
- For the first frontend version, a card's `closingDay` can be treated and displayed as the spreadsheet's card due day (`VCTO.`).
- Shared receivables should be included in the dashboard, not hidden only under people/share management.
- The label `Recebidos de alguém` should be kept for familiarity, but the product meaning is values to receive from someone for the month, not only values already paid.
- The first version should prioritize monthly scanning over editing every entity inline.
- "Delete and recreate" remains the correction workflow for expenses until the postponed expense update items are brought back into scope.
- The first frontend can keep card details lightweight: show linked card identity and due date context, then link to card management for edits.
- Fixed expenses are not currently a dedicated backend feature. For the initial frontend, they should be treated as normal expenses and grouped through frontend/product convention until a first-class fixed-expense concept is intentionally added.

## Design Implications

- Use compact tables/lists rather than large marketing-style cards.
- Use summary metrics for top-level totals.
- Use clear financial colors:
  - income/positive;
  - expense/negative;
  - shared receivable;
  - remaining balance.
- Keep group headers strong enough to replace spreadsheet blocks.
- Preserve high information density, but improve readability with spacing, alignment, filtering, and responsive behavior.
- Money values should align consistently for fast comparison.
- Installment progress and due dates should be visually scannable.

## Open Questions

- Should monthly shared receivables expose installment progress directly to avoid extra frontend joins?

## Resolved Questions

- The first dashboard should follow the spreadsheet closely at first, using familiar grouped sections.
- Fixed expenses are not currently modeled as a dedicated feature; they remain normal expenses for now.
- `closingDay` can represent the card due day in the first frontend.
- `Recebidos de alguém` means values to receive from someone in the month. The current backend summary already matches this by using all shared receivables due in the month regardless of payment status.

## Shared Receivable Installment Progress

The remaining open question is about the `4/10` style value shown beside shared receivables in the spreadsheet.

Example:

- `Beltrano - compra parcelada B`
- amount owed for this month
- installment progress such as `4/10`

The monthly share receivables endpoint, `GET /api/expense-share-installments/monthly`, currently tells the frontend who owes money, for which expense, how much, the due date, and whether it was paid. It does not directly include "this receivable belongs to installment 4 of 10".

This does not block the first dashboard. The frontend can initially show the person, expense, amount, due date, and paid state. If the `4/10` display becomes important for the first dashboard, a future backend read-model improvement should expose the expense installment number and total installments directly on monthly shared receivable rows.
