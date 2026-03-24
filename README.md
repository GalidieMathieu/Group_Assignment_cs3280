# Group Assignment Invoice System

This project is a WPF invoice system for `Weber Accounting`.
It uses a Microsoft Access database as the backend.

Right now, this project includes:

- the `Main` window
- the `Search` window
- shared database connection code
- basic WPF styles
- seeded fake data in the Access database for testing

The `Items` folder is included to match the assignment structure, but that window is not implemented yet.

## What Has Been Done

- created the Access database file
- created the main invoice window
- created the search window
- separated UI, logic, SQL, models, and database connection code
- added fake data for testing
- added a simple WPF theme

## Project Structure

```text
Group_Assignment/
|-- App.xaml
|-- App.xaml.cs
|-- Group_Assignment.csproj
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

## Where Everything Is

### `Main`

Files for the main invoice window:

- `wndMain.xaml` = UI
- `wndMain.xaml.cs` = window event handling
- `clsMainLogic.cs` = main window logic
- `clsMainSQL.cs` = SQL for the main window

### `Search`

Files for the search window:

- `wndSearch.xaml` = UI
- `wndSearch.xaml.cs` = window event handling
- `clsSearchLogic.cs` = search logic
- `clsSearchSQL.cs` = SQL for the search window

### `Models`

Data objects used in the app:

- `clsInvoiceSummary.cs`
- `clsInvoiceLine.cs`
- `clsItemDefinition.cs`

### `Data`

Shared database connection code:

- `clsDatabaseConnection.cs`
- `clsDatabaseSettings.cs`

### `Database`

Contains the Access database file:

```text
Database/WeberAccounting.accdb
```

## Database Structure

The Access database uses 3 tables:

### `Invoices`

Stores the invoice header.

- `InvoiceNumber` = primary key
- `InvoiceDate`
- `TotalCharge`

### `InvoiceItems`

Stores the items on each invoice.

- `InvoiceItemId` = primary key
- `InvoiceNumber` = links to `Invoices`
- `ItemCode`
- `ItemDescription`
- `ItemCost`

### `ItemDefinitions`

Stores the default item list used in the combo box.

- `ItemCode` = primary key
- `ItemDescription`
- `ItemCost`

## Database Architecture

The table relationship is:

```text
Invoices.InvoiceNumber 1 ---- many InvoiceItems.InvoiceNumber
```

`ItemDefinitions` is the master item list.
When an item is added to an invoice, the item code, description, and cost are saved into `InvoiceItems`.

## Current Database Data

The database already contains fake test data:

- 6 item definitions
- 3 invoices
- 7 invoice item rows

## Notes

- The main and search windows are implemented.
- The items window is not implemented yet.
- The app uses a simple MVVM-style separation:
  - XAML files = UI
  - Logic classes = window logic
  - Models = data objects
  - Data classes = database connection
