# Frontend V1 Scope

This document replaces open-ended frontend exploration with a shipping-first V1 scope.

The current goal is to finish a real frontend that is:

- usable by the user and close family;
- strong enough to present in a portfolio;
- modern enough to look intentional;
- simple enough to finish without months of design churn.

## Product Goal

Fenix V1 should let an authenticated user answer the monthly question fast:

> What came in, what is still receivable, what is going out this month, and how much is left?

The app does not need to look perfect. It needs to look coherent, feel trustworthy, and support the real monthly workflow without depending on the spreadsheet.

## V1 Design Direction

- Modern, but restrained.
- Finance workbench, not marketing site.
- Expense-focused center of gravity.
- Spreadsheet mental model preserved, spreadsheet visuals not copied literally.
- Calm neutrals by default, semantic colors only where they add meaning.

The saved reference for the more ambitious facelift remains useful:

- `docs/frontend-preview/dashboard-preview-modern-reference.html`

But V1 should simplify before implementing.

### Approved V1 Preview

The approved visual reference for the current V1 direction is:

- `docs/frontend-preview/dashboard-preview.html`

Supporting references kept for context:

- `docs/frontend-preview/dashboard-preview-modern-reference.html`
- `docs/frontend-preview/dashboard-preview-spreadsheet-reference.html`

Important product rule:

- `Recebidos de alguém` stays visually and semantically separate from `Entradas` in the summary layer.
- Reason: a receivable may correspond to reimbursement for an expense already carried by the user, so it should not be treated as ordinary disposable income.

## Primary Screen

V1 revolves around one strong monthly page.

### Monthly Page Anatomy

1. Top summary row
   - `Entradas`
   - `Recebidos de alguém`
   - `Despesas`
   - `Sobra`
2. Main expense work area
   - unified monthly expense list as the primary surface;
   - quick filters;
   - visible recurring/fixed markers;
   - visible installment progress;
   - visible card context where applicable.
3. Card statements section
   - grouped by card and due date;
   - simple breakdown of purchases and installments.
4. Secondary monthly context
   - compact `Entradas` breakdown;
   - compact `Recebidos de alguém` breakdown.

## Deliberate Simplifications

These are intentional V1 constraints, not missing polish.

- No chart-heavy dashboard.
- No advanced contextual sidebar required for first release.
- No attempt to recreate the spreadsheet cell by cell.
- No inline editing everywhere.
- No large design-system expansion before the app is alive.
- No special layout experiments that are hard to explain or hard to implement.

If a UI idea takes too much explanation to justify, it probably does not belong in V1.

## V1 Feature Scope

### Must Have

- frontend app foundation in `src/web`;
- authentication flow with cookie-backed session;
- protected app shell;
- monthly dashboard page;
- month navigation;
- read-only monthly summary;
- read-only monthly expenses;
- read-only monthly incomes;
- read-only monthly shared receivables;
- create and delete income;
- create and delete expense;
- create and manage cards enough to support expense entry;
- create and manage people enough to support shared-expense entry;
- clear loading, empty, and error states.

### Should Have

- modal-based create flows instead of full-page CRUD;
- dashboard refresh after mutations;
- clear card labels and due-date context;
- clear shared-expense cues;
- responsive behavior that stays usable on smaller laptop widths.

### Not Required For V1

- editing every entity before release;
- advanced filtering and saved views;
- analytics or charts;
- animation-heavy transitions;
- theming/dark mode;
- a polished mobile-first redesign;
- perfect component abstraction before real usage exists.

## UX Rules For V1

- The monthly dashboard must be understandable in under a minute.
- The expense list must be easier to scan than the spreadsheet, not fancier than the spreadsheet.
- `Entradas` and `Recebidos de alguém` must remain visible but secondary.
- The user should not need to understand special UI patterns to do basic monthly work.
- Create flows should be boring and clear.

## Implementation Shape

### Layout

- Simple authenticated app shell.
- Top navigation or very light app navigation.
- Main content width optimized for monthly work, not empty whitespace.

### Interactions

- Dialogs for create and delete confirmation.
- Simple buttons and filter chips.
- Minimal hidden state.
- Prefer explicit sections over clever context panels.

### Visual Priority

1. money values;
2. section labels;
3. installment/fixed/share context;
4. supporting helper text.

## Delivery Order

1. Scaffold `src/web`.
2. Add global styles, routing, query provider, and app shell.
3. Implement authentication.
4. Build monthly dashboard read path first.
5. Add mutation flows needed for real use:
   - incomes
   - expenses
   - cards
   - people
6. Add final presentation polish and portfolio cleanup.

## Definition Of Done

Frontend V1 is done when the user can:

- log in;
- open the monthly dashboard;
- switch month;
- understand the monthly totals quickly;
- see expenses, cards, incomes, and shared receivables clearly;
- create and delete the records needed for normal monthly use;
- use the app for real monthly tracking without falling back to the spreadsheet for core workflow.

Portfolio quality is achieved when the app also:

- looks deliberate and consistent;
- has a visible design point of view;
- demonstrates practical frontend architecture;
- avoids unfinished experimental interaction patterns.
