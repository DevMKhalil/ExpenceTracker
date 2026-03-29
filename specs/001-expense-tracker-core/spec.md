# Feature Specification: Expense Tracker Core

**Feature Branch**: `001-expense-tracker-core`  
**Created**: 2026-03-27  
**Status**: Draft  
**Input**: User description: "Expense tracker application with badge management, expense entry with badges/date/importance/notes, dashboard with daily/monthly/badge views, and quick-entry UI"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Badge Management (Priority: P1)

As a user, I want to create, view, edit, and delete badges so I can categorize my expenses with meaningful, color-coded labels (e.g., "Food", "Transport", "Entertainment").

Each badge has a name and a unique color chosen by the user. Badges serve as reusable tags that can be attached to any expense.

**Why this priority**: Badges are the foundational categorization mechanism. Without badges, expenses cannot be categorized, and the dashboard cannot group by badge. This must exist before expenses can be tagged.

**Independent Test**: Can be fully tested by creating, viewing, editing, and deleting badges. Delivers value by establishing the category system for the entire application.

**Acceptance Scenarios**:

1. **Given** the user is on the badges page, **When** they create a new badge with name "Food" and color green, **Then** the badge appears in the badge list with the correct name and color.
2. **Given** a badge "Transport" exists, **When** the user edits its name to "Commute" and changes its color, **Then** the badge list reflects the updated name and color.
3. **Given** a badge "Old Category" exists with no expenses attached, **When** the user deletes it, **Then** the badge is removed from the list.
4. **Given** a badge has expenses attached, **When** the user deletes it, **Then** the system warns the user, and upon confirmation, soft-deletes the badge (hidden from lists but preserved on existing expenses).
5. **Given** the user is creating a badge, **When** they try to save without a name, **Then** the system shows a validation error requiring a name.
6. **Given** the user is on the badges page, **When** they view the list, **Then** each badge displays its name with its assigned color clearly visible.

---

### User Story 2 - Expense Entry (Priority: P1)

As a user, I want to quickly add an expense with a name, amount, date, importance level, one or more badges, optional notes, and a pending status so I can accurately record my spending as it happens.

The entry form defaults to the current date and time but allows manual date selection. Importance levels are: Normal, Important, and Very Important. The user can attach multiple badges to a single expense. A "Pending" checkbox allows marking expenses that are not yet finalized.

**Why this priority**: This is the core functionality — recording expenses. Without expense entry, the application has no data and no purpose. Equal priority with badges since both are required for a functional MVP.

**Independent Test**: Can be fully tested by adding expenses with various combinations of fields and verifying they are saved correctly. Delivers the primary value of the application — tracking spending.

**Acceptance Scenarios**:

1. **Given** the user opens the expense entry form, **When** the form loads, **Then** the date field defaults to the current date and time.
2. **Given** the user is adding an expense, **When** they fill in name "Lunch", amount 150, select badge "Food", and leave the date as current, **Then** the expense is saved successfully with all provided details.
3. **Given** the user is adding an expense, **When** they select multiple badges ("Food" and "Work"), **Then** the expense is saved with both badges attached.
4. **Given** the user is adding an expense, **When** they set the importance to "Very Important", **Then** the saved expense reflects the chosen importance level.
5. **Given** the user is adding an expense, **When** they check the "Pending" checkbox, **Then** the expense is saved with pending status and visually distinguished in lists.
6. **Given** the user is adding an expense, **When** they manually change the date to a past date, **Then** the expense is saved with the manually entered date.
7. **Given** the user is adding an expense, **When** they submit without a name or amount, **Then** the system shows validation errors for the required fields.
8. **Given** the user is adding an expense, **When** they add optional notes, **Then** the notes are saved and visible when viewing the expense details.

---

### User Story 3 - Expense List & Management (Priority: P2)

As a user, I want to view, edit, and delete my recorded expenses so I can manage and correct my spending records as needed.

**Why this priority**: After entering expenses, users need the ability to review and correct them. This completes the CRUD cycle for expenses and is essential for maintaining accurate records.

**Independent Test**: Can be tested by creating expenses and then viewing, editing, and deleting them. Delivers value by enabling users to maintain accurate records.

**Acceptance Scenarios**:

1. **Given** expenses exist, **When** the user views the expense list, **Then** expenses are displayed with name, amount, date, badges (shown in their colors), importance level, and pending status.
2. **Given** an expense exists, **When** the user edits its amount from 150 to 200, **Then** the updated amount is saved and reflected in the list.
3. **Given** an expense exists, **When** the user deletes it, **Then** the expense is removed from the list.
4. **Given** the expense list is displayed, **When** there are many expenses, **Then** the list supports a "Load More" button pagination approach to remain usable.
5. **Given** the user views an expense, **When** it is marked as "Pending", **Then** it is visually distinguished from finalized expenses.

---

### User Story 4 - Dashboard Overview (Priority: P2)

As a user, I want a dashboard that shows my spending summaries — daily totals, monthly totals, and breakdowns by badge — so I can understand my spending patterns at a glance.

**Why this priority**: The dashboard provides the analytical value of the application. While expenses can be tracked without it, understanding spending patterns is a key motivator for users.

**Independent Test**: Can be tested by adding known expenses and verifying that the dashboard accurately reflects daily totals, monthly totals, and badge breakdowns. Delivers insight and analytical value.

**Acceptance Scenarios**:

1. **Given** the user has recorded non-pending expenses today, **When** they open the dashboard, **Then** they see today's total spending amount (excluding pending expenses).
2. **Given** the user has recorded non-pending expenses this month, **When** they view the monthly summary, **Then** they see the total spending for the current month (excluding pending expenses).
3. **Given** expenses exist with different badges, **When** the user views the badge breakdown, **Then** they see total spending per badge, displayed with each badge's color (excluding pending expenses).
4. **Given** no expenses exist for today, **When** the user views the daily summary, **Then** the dashboard shows zero or a "no expenses" message.
5. **Given** multiple months of expenses exist, **When** the user views the dashboard, **Then** they see the current month's summary only. There is no historical month navigation; the dashboard always shows the current month.
6. **Given** the user has pending expenses, **When** they view the dashboard, **Then** a separate pending total is displayed alongside the main totals.

---

### User Story 5 - Quick Entry UI (Priority: P3)

As a user, I want the expense entry interface to be streamlined and optimized for speed so I can record expenses in just a few seconds without unnecessary steps.

**Why this priority**: Usability enhancement. The core entry functionality (Story 2) must work first; this story focuses on making it faster and more convenient for daily use.

**Independent Test**: Can be tested by timing how quickly a user can add an expense and verifying that common shortcuts and defaults reduce entry friction. Delivers improved user experience and encourages consistent tracking.

**Acceptance Scenarios**:

1. **Given** the user opens the expense entry form, **When** all defaults are pre-filled (current date, Normal importance, Pending unchecked), **Then** the user only needs to fill in name, amount, and optionally select badges.
2. **Given** the user has just saved an expense, **When** the save completes, **Then** the form resets and is ready for the next entry immediately.
3. **Given** the user is entering an expense, **When** they interact with the form, **Then** keyboard navigation between fields is smooth and logical (tab order).
4. **Given** the user is selecting badges, **When** they click/tap badges, **Then** badges toggle on/off quickly without opening a separate dialog.

---

### Edge Cases

- What happens when a user tries to enter a negative amount? The system should reject negative values and show a validation error.
- What happens when a user enters a future date? The system should allow future dates (for planned expenses) but visually indicate them.
- What happens when all badges are deleted but expenses with those badges exist? Soft-deleted badges remain visible on their historical expenses (with their original name and color) but cannot be selected for new expenses.
- What happens when the user enters a very large amount? The system MUST show a validation error for values exceeding the maximum limit defined in FR-006 (99,999,999.99).
- What happens when the user creates a badge with a name that already exists? The system should prevent duplicate badge names.

## Requirements *(mandatory)*

### Functional Requirements

**Badge Management**:
- **FR-001**: System MUST allow users to create a badge with a name and a user-selected color.
- **FR-002**: System MUST allow users to view all badges in a list, each displayed with its assigned color.
- **FR-003**: System MUST allow users to edit a badge's name and color.
- **FR-004**: System MUST allow users to delete a badge. If expenses reference it, the badge is soft-deleted: hidden from badge selection and management lists, but preserved on existing expenses for historical accuracy. Soft-deleted badges on historical expenses MUST be displayed with reduced opacity and a "deleted" indicator. A confirmation prompt is shown before proceeding.
- **FR-005**: System MUST enforce unique badge names (case-insensitive).

**Expense Entry**:
- **FR-006**: System MUST allow users to create an expense with: name (required), amount (required, positive number, maximum 99,999,999.99), date, importance level, badges, notes, and pending status.
- **FR-007**: System MUST default the expense date to the current date and time when creating a new expense.
- **FR-008**: System MUST allow users to manually set or change the expense date.
- **FR-009**: System MUST provide three importance levels: Normal, Important, and Very Important.
- **FR-010**: System MUST default the importance level to "Normal" for new expenses.
- **FR-011**: System MUST allow attaching one or more badges to an expense.
- **FR-012**: System MUST allow users to add optional free-text notes to an expense.
- **FR-013**: System MUST provide a "Pending" checkbox that defaults to unchecked.
- **FR-014**: System MUST allow users to edit any field of an existing expense.
- **FR-015**: System MUST allow users to delete an expense.

**Dashboard**:
- **FR-016**: System MUST display a daily spending total for the current day, excluding pending expenses.
- **FR-017**: System MUST display a monthly spending total for the current month, excluding pending expenses.
- **FR-018**: System MUST display a spending breakdown grouped by badge, showing each badge's total with its assigned color, excluding pending expenses.
- **FR-024**: System MUST display a separate pending total on the dashboard showing the sum of all pending expenses regardless of time range.

**Quick Entry UX**:
- **FR-019**: System MUST reset the expense form after a successful save, ready for the next entry.
- **FR-020**: System MUST support logical tab-order navigation through form fields.
- **FR-021**: Badge selection MUST use an inline toggle mechanism (no separate dialog required).

**Localization & Layout**:
- **FR-022**: System MUST default to Arabic with RTL layout direction, and dynamically switch to LTR when English is active.
- **FR-023**: All user-facing text (labels, placeholders, validation messages, dashboard text) MUST be localized via resource files (.resx) and rendered in the active locale (Arabic or English).
- **FR-025**: System MUST visually indicate future-dated expenses by displaying a distinct "Future" badge and a subtle yellow background highlight so users can distinguish planned expenses from past ones.

**Accessibility**:
- **FR-026**: System MUST meet WCAG 2.1 AA compliance — semantic HTML, ARIA labels on interactive elements, keyboard navigation, and minimum 4.5:1 contrast ratio for normal text.

**Expense List Behavior**:
- **FR-027**: System MUST default the expense list sort order to Date descending (newest first).

### Key Entities

- **Badge**: A reusable category label for expenses. Has a unique name, a user-chosen color, and an IsDeleted flag (for soft-delete). Can be associated with many expenses. Soft-deleted badges remain on historical expenses but are hidden from selection.
- **Expense**: A spending record. Has a name, monetary amount (stored as decimal with 2-place precision, no currency symbol), date/time, importance level (Normal / Important / Very Important), optional notes, and a pending flag. Can be associated with one or more badges.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can create a new badge in under 30 seconds.
- **SC-002**: Users can record a basic expense (name, amount, one badge) in under 20 seconds.
- **SC-003**: Users can view their daily and monthly spending totals within 2 seconds of opening the dashboard.
- **SC-004**: The dashboard badge breakdown is sorted by total descending, placing the highest spending category first.
- **SC-005**: After saving an expense, the next entry can begin with zero delay (form resets per FR-019).
- **SC-006**: All CRUD operations for badges and expenses complete with visible feedback within 2 seconds.
- **SC-007**: The expense entry form requires filling only 2 fields (name and amount) with all other fields pre-defaulted, enabling first-time users to add an expense without guidance.

## Clarifications

### Session 2026-03-28

- Q: What is the primary UI language and layout direction? → A: Arabic (RTL layout)
- Q: What data persistence mechanism should be used? → A: JSON files (with EF Core-ready structure for future swap to SQL Server)
- Q: What happens to expenses when a badge with expenses is deleted? → A: Soft-delete — badge is hidden from selection but preserved on existing expenses for historical accuracy
- Q: What currency and decimal precision for expense amounts? → A: No specific currency symbol; display numbers with 2 decimal places
- Q: Should pending expenses be included in dashboard totals? → A: Exclude from main totals; show a separate "pending total" on dashboard

## Assumptions

- The application UI is in Arabic with right-to-left (RTL) layout direction. All labels, validation messages, and user-facing text are in Arabic.
- The application is single-user (no authentication or multi-user support required in this version).
- The application will store data locally using JSON files as the default persistence provider, with an EF Core-ready structure allowing future swap to SQL Server via configuration. Cloud sync is out of scope for this version.
- Currency is uniform — the system does not display a currency symbol and does not handle currency conversion. All amounts are stored and displayed as plain numbers with 2 decimal places.
- The application is web-based, running in a browser. Mobile-native versions are out of scope.
- Badge colors are chosen from a predefined palette or a color picker; the exact mechanism is an implementation detail.
- "Pending" expenses are excluded from the main dashboard totals (daily, monthly, by-badge). The dashboard displays a separate pending total so users can distinguish confirmed spending from planned/uncertain spending.
- The existing ASP.NET project structure will be used as the foundation for this feature.
