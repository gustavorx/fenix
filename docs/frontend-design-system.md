# Frontend Design System

This document defines the first shared design-system layer for Fenix before `src/web` is scaffolded.

It translates the frontend backlog, architecture, and product notes into reusable visual rules so the first dashboard and forms can be built from shared tokens and components instead of one-off styling.

V1 note: this design system should support shipping a coherent app quickly. If a pattern is elegant in theory but slows implementation materially, prefer the simpler pattern for V1.

## Design Principles

- Dashboard-first. The first screen is a monthly finance workbench, not a marketing page.
- Dense enough for scanning. The UI should show meaningful finance data without wasting space, while still preserving clear grouping and readable spacing.
- Calm hierarchy. Neutrals carry most of the interface. Accent and status colors are reserved for meaning, not decoration.
- Financial states are explicit. Income, expenses, receivables, warnings, and destructive actions must be visually distinguishable at a glance.
- Repeated monthly work should feel fast. Common actions such as changing month, adding records, confirming deletes, and scanning totals should require minimal friction.
- Spreadsheet language stays visible. Labels such as `Entradas`, `Despesas`, `Sobra`, `Débito / Outros`, and card due-date cues should remain close to the user's current mental model.

## Visual Direction

Fenix should default to a light, desk-like interface:

- cool neutral page background;
- white and off-white surfaces;
- slate and navy text for trust and readability;
- restrained blue accent for primary actions and focus;
- semantic green, rose, teal, and amber for finance states.

The first version should be table-first and list-first, not chart-first. Summary cards are useful, but the real product value comes from structured monthly sections that can be scanned quickly.

Avoid these anti-patterns in the initial UI:

- hero-section or landing-page styling;
- oversized cards with excessive padding;
- decorative gradients, glass effects, or heavy shadows;
- default dark theme;
- hiding finance meaning behind generic neutral chips or unlabeled icons.
- clever contextual layouts that are harder to explain than a normal section.

## Typography

### Font Family

- Primary UI font: `IBM Plex Sans`
- Fallback stack: `system-ui`, `sans-serif`

Reasoning:

- it reads well in dense dashboards;
- it feels trustworthy without looking overly corporate;
- numeric data remains legible in tables, metrics, and forms.

### Numeric Rules

- Money, percentages, due days, and installment progress should use tabular numerals.
- Summary totals should increase weight before they increase size.
- Long labels should wrap cleanly instead of forcing oversized metric cards.

### Type Scale

| Token | Size | Usage |
| --- | --- | --- |
| `text-xs` | 12px | helper text, small metadata, installment suffixes |
| `text-sm` | 14px | table cells, compact labels, chips |
| `text-base` | 16px | default body text, inputs, dialogs |
| `text-lg` | 18px | section headers, emphasized list rows |
| `text-xl` | 20px | card titles, modal titles |
| `text-2xl` | 24px | summary metric values on small screens |
| `text-3xl` | 30px | primary monthly totals on desktop |

Use medium and semibold weight more often than large jumps in size.

## Color Tokens

These are semantic tokens for the first implementation. Hex values are intentionally conservative so the app feels stable during long monthly use.

| Token | Hex | Usage |
| --- | --- | --- |
| `bg.canvas` | `#F8FAFC` | app background |
| `bg.subtle` | `#F1F5F9` | grouped areas, soft row highlights |
| `bg.surface` | `#FFFFFF` | cards, panels, dialogs |
| `bg.surface-muted` | `#F8FAFC` | input backgrounds, nested surfaces |
| `brand.primary` | `#1D4ED8` | primary actions, active controls, links |
| `brand.primary-soft` | `#DBEAFE` | selected states, informational chips |
| `text.primary` | `#0F172A` | main text and values |
| `text.secondary` | `#334155` | supporting text, section labels |
| `text.muted` | `#64748B` | helper text, empty-state support copy |
| `text.inverse` | `#F8FAFC` | text on dark or strong semantic fills |
| `border.default` | `#E2E8F0` | default borders and dividers |
| `border.strong` | `#CBD5E1` | emphasized boundaries, active table groups |
| `status.income` | `#15803D` | income totals, positive finance values |
| `status.income-soft` | `#DCFCE7` | soft positive badges and surfaces |
| `status.expense` | `#BE123C` | expense emphasis and negative outflow values |
| `status.expense-soft` | `#FFE4E6` | soft expense badges and highlights |
| `status.receivable` | `#0F766E` | values to receive from someone |
| `status.receivable-soft` | `#CCFBF1` | shared receivable badges and row accents |
| `status.warning` | `#B45309` | warnings, pending attention, incomplete setup |
| `status.warning-soft` | `#FEF3C7` | warning backgrounds |
| `status.destructive` | `#B91C1C` | delete actions and irreversible confirmations |
| `status.destructive-soft` | `#FEE2E2` | destructive callouts |
| `focus.ring` | `#2563EB` | keyboard focus and active field ring |

### Suggested CSS Variable Mapping

These names fit the planned Tailwind and selective shadcn/ui setup:

```css
:root {
  --background: #F8FAFC;
  --surface: #FFFFFF;
  --surface-muted: #F8FAFC;
  --foreground: #0F172A;
  --muted-foreground: #64748B;
  --border: #E2E8F0;
  --ring: #2563EB;
  --primary: #1D4ED8;
  --primary-foreground: #F8FAFC;
  --income: #15803D;
  --income-soft: #DCFCE7;
  --expense: #BE123C;
  --expense-soft: #FFE4E6;
  --receivable: #0F766E;
  --receivable-soft: #CCFBF1;
  --warning: #B45309;
  --warning-soft: #FEF3C7;
  --destructive: #B91C1C;
  --destructive-foreground: #F8FAFC;
}
```

## Radius And Shadow Tokens

| Token | Value | Usage |
| --- | --- | --- |
| `radius.sm` | 8px | inputs, chips, compact buttons |
| `radius.md` | 12px | cards, dialogs, dropdown panels |
| `radius.lg` | 16px | larger section containers when needed |
| `shadow.sm` | `0 1px 2px rgba(15, 23, 42, 0.06)` | subtle separation for panels |
| `shadow.md` | `0 8px 24px rgba(15, 23, 42, 0.08)` | dialogs and elevated overlays |

Use shadow sparingly. Borders should do most of the structural work.

## Spacing And Layout Scale

Use a 4px base scale.

| Token | Value | Usage |
| --- | --- | --- |
| `space-1` | 4px | tight chip padding, icon gaps |
| `space-2` | 8px | row metadata gaps, helper text spacing |
| `space-3` | 12px | compact form spacing |
| `space-4` | 16px | default card padding on mobile |
| `space-5` | 20px | grouped content spacing |
| `space-6` | 24px | default desktop card padding, page gaps |
| `space-8` | 32px | section separation |
| `space-10` | 40px | major page sections |

Layout rules:

- Page padding: 16px mobile, 24px tablet, 32px desktop.
- Summary cards should sit in a compact responsive grid, not a single long row on smaller screens.
- Monthly groups should read as stacked sections with strong titles and light internal dividers.
- Dense tables may scroll horizontally on smaller screens; do not squeeze columns until the content becomes ambiguous.

## Component Families

### Buttons

- Variants: `primary`, `secondary`, `ghost`, `destructive`.
- Default height: 40px. Compact height: 36px.
- Primary buttons use `brand.primary`.
- Secondary buttons use `bg.surface` with `border.default`.
- Ghost buttons are reserved for low-emphasis actions inside dense layouts.
- Destructive buttons use `status.destructive` and should appear only inside clearly risky flows.

### Icon Buttons

- Default size: 36x36.
- Use for month navigation, inline row actions, dismiss controls, and refresh.
- Icon-only actions must still expose accessible labels.
- Avoid using icon-only buttons for destructive actions when text would remove ambiguity.

### Inputs

- Height: 40px by default.
- Labels stay visible above the field; placeholder text is not a label replacement.
- Inputs use `bg.surface-muted`, `border.default`, and `focus.ring`.
- Dense forms should use consistent vertical rhythm instead of ad hoc margin tweaks.

### Money Input

- Money fields should prioritize numeric scanning:
  - tabular numerals;
  - right-aligned typed value;
  - optional `R$` prefix or leading visual currency label;
  - normalization to two decimals on blur.
- Error messaging should stay close to business language, not raw parsing language.

### Date Input

- Use native browser date input first unless product requirements prove it insufficient.
- Where the business meaning is important, pair the field with a short helper such as due-date context.
- Displayed dates in lists should remain compact and locale-appropriate.

### Month Selector

- The monthly dashboard should use a dedicated month selector rather than a generic date picker.
- Preferred composition:
  - previous month button;
  - current month and year trigger;
  - next month button.
- It should support quick month-to-month stepping because that is a primary workflow.

### Select

- Use a select component for cards, people, and other constrained lists.
- Start with the standard shadcn `Select` pattern for accessibility and keyboard support.
- Do not introduce searchable or async selects until the data size actually requires them.

### Checkbox Or Toggle

- Use checkboxes for persisted boolean choices in forms.
- Use toggles only for immediate interface state, such as compact view or paid/unpaid filters.
- Do not swap these roles casually; the interaction meaning should stay obvious.

### Form Field

- Standard anatomy:
  - label;
  - control;
  - optional helper text;
  - inline validation message.
- Required-field indication should be consistent across all forms.
- Errors should change border and message color, but the message must still be understandable without color alone.

### Dialog

- Use dialogs for focused creation, edit, and detail tasks that should not abandon dashboard context.
- Default dialog widths:
  - narrow confirmation: 400px to 440px;
  - standard form dialog: 480px to 640px.
- Dialog structure should remain consistent: title, optional description, body, footer actions.

### Confirmation Dialog

- Confirmations should name the record being affected and the consequence.
- For delete flows, default the primary action to a secondary visual treatment and keep the destructive action explicit.
- If a workflow is reversible, prefer normal confirmation copy over alarmist styling.

### Toast Or Notification

- Use toast notifications for mutation outcomes and lightweight reminders.
- Variants: success, error, warning, info.
- Toasts should be short, specific, and dismissible.
- Do not use toasts for validation that belongs inline in a form.

### Summary Metric

- Each metric card should include:
  - label;
  - primary amount;
  - optional secondary explanation or percentage context.
- `Entradas`, `Recebidos de alguém`, `Despesas`, and `Sobra` are first-class metrics.
- Money value is the visual priority; supporting text stays smaller and calmer.
- Use semantic color accents only where they add meaning, not on every metric by default.

### Data Table Or List

- Desktop uses semantic tables for truly tabular data.
- Mobile can collapse those rows into stacked cards when necessary.
- Right-align money columns.
- Use tabular numerals for money and installment progress.
- Grouped monthly sections should keep titles and totals visible before the row list begins.
- Hover states should be subtle and should never make finance status harder to read.

### Empty State

- Keep empty states short and practical:
  - what is missing;
  - why it matters right now;
  - the most relevant action.
- Avoid illustration-heavy empty states in the first version.

### Error State

- Show a concise explanation, preserve context when possible, and provide a retry action.
- Distinguish between loading failure, unauthorized session loss, and domain validation problems.
- Error states should not wipe known good data unless the data is truly unavailable.

### Loading State

- Prefer skeleton layouts when the shape of the final UI is known.
- Use inline or section-level loaders before full-page loaders.
- Avoid spinner-only loading for dashboard sections that can reserve layout in advance.

## Accessibility And Interaction Rules

- Meet at least WCAG AA contrast for text and essential UI states.
- Keep focus states visible and consistent across all interactive components.
- Respect `prefers-reduced-motion`.
- Never communicate finance meaning through color alone; pair color with labels, icons, or position.
- Interactive targets should remain comfortable on touch devices even when the layout is dense.

## First Dashboard Application

When implementation starts, this design system should directly guide:

- the top monthly summary metric row;
- grouped expense sections for fixed, card, and debit/other expenses;
- receivable rows tied to people and source expenses;
- month navigation;
- CRUD dialogs for incomes, cards, people, and expenses;
- shared feedback states across loading, empty, and mutation outcomes.

If a new screen cannot be expressed with these tokens and families, prefer extending this document before inventing a one-off visual pattern in code.
