# Group Assignment Invoice System

This project is a WPF invoice system for a small business named `Weber Accounting`.
The application uses a Microsoft Access database as the backend and follows the course requirement to separate:

- UI code
- SQL statements
- business logic
- shared database connection code

The project is organized using an `MVVM-style` structure adapted to the assignment requirements.
Because the professor requires the `Main` and `Search` folders and specific file names, the project does not use separate `Views` and `ViewModels` folders.
Instead, the MVVM responsibilities are mapped into the required structure.

At this stage, the project includes the `Main` and `Search` windows plus the shared database connection layer.
The `Items` folder is already reserved so the project structure matches the assignment requirements.

## Project Tree

The tree below shows the important source folders and what each file is responsible for.
Build folders such as `bin`, `obj`, `.vs`, and `.dotnet` are not included because they are generated automatically.

```text
Group_Assignment/
|-- App.xaml
|-- App.xaml.cs
|-- AssemblyInfo.cs
|-- Group_Assignment.csproj
|-- Group_Assignment.slnx
|-- README.md
|-- Data/
|   |-- clsDatabaseConnection.cs
|   `-- clsDatabaseSettings.cs
|-- Database/
|   |-- README.md
|   `-- WeberAccounting.accdb
|-- Items/
|   `-- README.md
|-- Main/
|   |-- clsMainLogic.cs
|   |-- clsMainSQL.cs
|   |-- wndMain.xaml
|   `-- wndMain.xaml.cs
|-- Models/
|   |-- clsInvoiceLine.cs
|   |-- clsInvoiceSummary.cs
|   `-- clsItemDefinition.cs
|-- Search/
|   |-- clsSearchLogic.cs
|   |-- clsSearchSQL.cs
|   |-- wndSearch.xaml
|   `-- wndSearch.xaml.cs
`-- Styles/
    `-- Theme.xaml
```

## Folder Guide

## MVVM Structure

This project follows an `MVVM-style` WPF design.

MVVM stands for:

- `Model`
- `View`
- `ViewModel`

In WPF, MVVM helps keep the UI separate from logic and data access.
That makes the code easier to test, easier to maintain, and easier to divide between team members.

### How MVVM Maps To This Project

Because this assignment requires specific folders and file names, the MVVM pattern is applied like this:

- `View`
  - `Main/wndMain.xaml`
  - `Main/wndMain.xaml.cs`
  - `Search/wndSearch.xaml`
  - `Search/wndSearch.xaml.cs`
- `ViewModel / presentation logic`
  - `Main/clsMainLogic.cs`
  - `Search/clsSearchLogic.cs`
- `Model`
  - files in the `Models` folder
- `Data access`
  - files in the `Data` folder
  - `clsMainSQL.cs`
  - `clsSearchSQL.cs`

### What Each MVVM Part Does

#### View

The `View` is the WPF screen the user sees.
It contains XAML controls such as:

- menus
- buttons
- combo boxes
- date pickers
- data grids

The code-behind for each window should stay as small as possible.
Its job is to handle UI events and connect the screen to the logic layer.

#### ViewModel / Presentation Logic

The logic classes act like the `ViewModel` layer in this project.

- `clsMainLogic.cs`
- `clsSearchLogic.cs`

These classes contain the presentation logic and business rules for each screen.
They should decide what data to load, how totals are calculated, and how the UI state should behave.
They should not contain WPF layout code.

#### Model

The `Models` folder contains the data objects used by the application, such as:

- invoice summary rows
- invoice line rows
- item definition rows

These classes represent the business data moving between the UI and the database.

#### Data Access

The `Data` folder contains the shared database connection code.
The SQL classes contain query text only.

This separation is important:

- `Data/clsDatabaseConnection.cs` handles connecting to Access
- `Data/clsDatabaseSettings.cs` stores database settings
- `Main/clsMainSQL.cs` stores main window SQL statements
- `Search/clsSearchSQL.cs` stores search window SQL statements

This keeps raw SQL and database setup out of the UI files.

### Why This Still Counts As MVVM

Even though the project does not use folders literally named `Views` and `ViewModels`, the responsibilities are still separated in the MVVM way:

- XAML windows are the `View`
- logic classes are the `ViewModel-style` layer
- model classes are the `Model`
- database classes are the `Data access` layer

That approach fits both:

- professional WPF design ideas
- the assignment's required file and folder structure

### Root Files

- `App.xaml`
  Starts the application and loads the shared theme.
- `App.xaml.cs`
  Application startup code-behind.
- `Group_Assignment.csproj`
  WPF project file. Targets `.NET 10` and references `System.Data.OleDb`.
- `README.md`
  Main documentation for the project.

### `Data`

- `clsDatabaseSettings.cs`
  Stores shared database paths and preferred Access providers.
- `clsDatabaseConnection.cs`
  Builds the Access connection string, tests the connection, and creates `OleDbConnection` objects.

This folder is the shared database layer used by both windows.

### `Main`

- `wndMain.xaml`
  Main invoice screen UI. This is part of the `View` layer.
- `wndMain.xaml.cs`
  Main window event wiring only. This should stay thin and UI-focused.
- `clsMainSQL.cs`
  SQL statements used by the main window.
- `clsMainLogic.cs`
  Main presentation logic. This acts like the `ViewModel` layer for the main screen.

This folder is responsible for creating or viewing invoices.

### `Search`

- `wndSearch.xaml`
  Search screen UI. This is part of the `View` layer.
- `wndSearch.xaml.cs`
  Search window event wiring only. This should stay thin and UI-focused.
- `clsSearchSQL.cs`
  SQL statements used by the search window.
- `clsSearchLogic.cs`
  Search presentation logic. This acts like the `ViewModel` layer for the search screen.

This folder is responsible for finding an existing invoice and returning it to the main window.

### `Models`

- `clsInvoiceSummary.cs`
  Small invoice record used in the search grid.
- `clsInvoiceLine.cs`
  One line item displayed on an invoice.
- `clsItemDefinition.cs`
  One item from the definition table used in the item combo box.

These classes are the `Model` layer in the MVVM structure.

### `Styles`

- `Theme.xaml`
  Shared styles and colors for the WPF application.

This is where the application theme lives so visual properties are not hard-coded into each control.

### `Database`

- `README.md`
  Notes about the expected Access database file and schema.
- `WeberAccounting.accdb`
  The Microsoft Access database file used by the application.

The Access database file now exists in this folder:

```text
Database/WeberAccounting.accdb
```

### `Items`

- `README.md`
  Placeholder folder for the future Items window.

This folder exists now so the final project already follows the assignment folder naming requirement.

## Database Overview

The current scaffold expects one Access database file:

```text
Database/WeberAccounting.accdb
```

The design uses three tables:

- `Invoices`
- `InvoiceItems`
- `ItemDefinitions`

The database currently includes fake sample data for testing:

- 6 item definitions
- 3 sample invoices
- 7 invoice line items

## Table Design

### 1. `Invoices`

Stores one row per invoice.

| Field Name | Data Type | Key | Description |
|---|---|---|---|
| `InvoiceNumber` | AutoNumber | Primary Key | Unique invoice number generated by Access |
| `InvoiceDate` | Date/Time | No | Date selected by the user |
| `TotalCharge` | Currency | No | Total amount of the invoice |

Example:

| InvoiceNumber | InvoiceDate | TotalCharge |
|---|---|---|
| 1 | 3/20/2026 | 399.99 |
| 2 | 3/21/2026 | 600.00 |
| 3 | 3/22/2026 | 690.00 |

### 2. `InvoiceItems`

Stores the items that belong to each invoice.

| Field Name | Data Type | Key | Description |
|---|---|---|---|
| `InvoiceItemId` | AutoNumber | Primary Key | Unique row id for each invoice line |
| `InvoiceNumber` | Number | Foreign Key | Links to `Invoices.InvoiceNumber` |
| `ItemCode` | Short Text | No | Item code from the item definition table |
| `ItemDescription` | Short Text | No | Item description copied to the invoice |
| `ItemCost` | Currency | No | Item cost copied to the invoice |

Example:

| InvoiceItemId | InvoiceNumber | ItemCode | ItemDescription | ItemCost |
|---|---|---|---|---|
| 1 | 1 | TAX104 | Individual Tax Return Preparation | 249.99 |
| 2 | 1 | CONS10 | Small Business Consultation | 150.00 |
| 3 | 2 | BOOK01 | Monthly Bookkeeping Service | 325.00 |

### 3. `ItemDefinitions`

Stores the item definition table used by the item combo box.

| Field Name | Data Type | Key | Description |
|---|---|---|---|
| `ItemCode` | Short Text | Primary Key | Unique item code |
| `ItemDescription` | Short Text | No | Item name shown to the user |
| `ItemCost` | Currency | No | Default item price |

Example:

| ItemCode | ItemDescription | ItemCost |
|---|---|---|
| TAX104 | Individual Tax Return Preparation | 249.99 |
| PAY201 | Monthly Payroll Processing | 180.00 |
| BOOK01 | Monthly Bookkeeping Service | 325.00 |
| AUD300 | Internal Audit Review | 540.00 |

## Table Relationships

The table relationship should be:

```text
Invoices.InvoiceNumber 1 ---- many InvoiceItems.InvoiceNumber
ItemDefinitions.ItemCode is referenced by invoice items when items are selected
```

`ItemDefinitions` is the master item list.
`InvoiceItems` stores a copy of the item description and item cost at the time the invoice is created.
That is useful because item descriptions or prices in the definition table can change later.

## Example Invoice

Here is one full example of how the three tables work together.

### ItemDefinitions

| ItemCode | ItemDescription | ItemCost |
|---|---|---|
| TAX104 | Individual Tax Return Preparation | 249.99 |
| CONS10 | Small Business Consultation | 150.00 |
| RET501 | Quarterly Retainer Meeting | 95.00 |

### Invoices

| InvoiceNumber | InvoiceDate | TotalCharge |
|---|---|---|
| 1 | 3/20/2026 | 399.99 |

### InvoiceItems

| InvoiceItemId | InvoiceNumber | ItemCode | ItemDescription | ItemCost |
|---|---|---|---|---|
| 1 | 1 | TAX104 | Individual Tax Return Preparation | 249.99 |
| 2 | 1 | CONS10 | Small Business Consultation | 150.00 |

Invoice `1` total:

```text
249.99 + 150.00 = 399.99
```

## How The Code Uses The Database

- The main window loads item choices from `ItemDefinitions`.
- When a saved invoice is opened, the main window loads the header from `Invoices` and the lines from `InvoiceItems`.
- The search window reads invoice summaries from `Invoices`.
- The shared database classes in `Data` handle the Access connection string and provider selection.

## Current Status

Implemented now:

- project folder structure
- shared Access connection classes
- main window scaffold
- search window scaffold
- main window opening the search window
- shared theme/styles

Not implemented yet:

- actual insert/update/delete invoice saving
- the `Items` management window
- final validation rules for all user inputs

## Notes For Access Setup

The connection helper currently tries these providers in this order:

- `Microsoft.ACE.OLEDB.16.0`
- `Microsoft.ACE.OLEDB.12.0`

If the database file exists but the ACE provider is not installed on the machine, the app will show a connection status message instead of crashing.
