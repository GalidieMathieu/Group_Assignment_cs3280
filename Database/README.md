# Database Folder

This folder now contains the Access database file used by the WPF project:

```text
WeberAccounting.accdb
```

## Current Database Contents

The database file has already been created with these tables:

- `Invoices`
- `InvoiceItems`
- `ItemDefinitions`

The database also includes fake sample data for testing the main and search windows.

## Table Summary

### `Invoices`

- `InvoiceNumber` - AutoNumber, Primary Key
- `InvoiceDate` - Date/Time
- `TotalCharge` - Currency

Example:

| InvoiceNumber | InvoiceDate | TotalCharge |
|---|---|---|
| 1 | 3/20/2026 | 399.99 |
| 2 | 3/21/2026 | 600.00 |

### `InvoiceItems`

- `InvoiceItemId` - AutoNumber, Primary Key
- `InvoiceNumber` - Number, Foreign Key to `Invoices.InvoiceNumber`
- `ItemCode` - Short Text
- `ItemDescription` - Short Text
- `ItemCost` - Currency

Example:

| InvoiceItemId | InvoiceNumber | ItemCode | ItemDescription | ItemCost |
|---|---|---|---|---|
| 1 | 1 | TAX104 | Individual Tax Return Preparation | 249.99 |
| 2 | 1 | CONS10 | Small Business Consultation | 150.00 |

### `ItemDefinitions`

- `ItemCode` - Short Text, Primary Key
- `ItemDescription` - Short Text
- `ItemCost` - Currency

Example:

| ItemCode | ItemDescription | ItemCost |
|---|---|---|
| TAX104 | Individual Tax Return Preparation | 249.99 |
| PAY201 | Monthly Payroll Processing | 180.00 |

## How The Tables Work Together

- `Invoices` stores the invoice header.
- `InvoiceItems` stores each line item on an invoice.
- `ItemDefinitions` stores the default items used in the item drop-down on the main window.

Relationship:

```text
Invoices.InvoiceNumber 1 ---- many InvoiceItems.InvoiceNumber
```

The application copies the selected item description and cost into `InvoiceItems`.
That preserves invoice history even if the definition table changes later.

## Seeded Data Note

The Access file now contains fake seeded data.
That lets the search screen and main screen load real rows immediately during testing.

## Provider Notes

The shared connection helper in the project tries these providers in order:

- `Microsoft.ACE.OLEDB.16.0`
- `Microsoft.ACE.OLEDB.12.0`
