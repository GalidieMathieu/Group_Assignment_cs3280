namespace Group_Assignment.Main
{
    /// <summary>
    /// Stores SQL statements used by the main window.
    /// </summary>
    public sealed class clsMainSQL
    {
        /// <summary>
        /// Gets the SQL that loads all item definitions for the invoice item drop-down.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetAllItemDefinitionsSql()
        {
            try
            {
                return
                    "SELECT ItemCode, ItemDescription, ItemCost " +
                    "FROM ItemDefinitions " +
                    "ORDER BY ItemCode;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the item definition SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that loads one invoice header.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetInvoiceHeaderByNumberSql()
        {
            try
            {
                return
                    "SELECT InvoiceNumber, InvoiceDate, TotalCharge " +
                    "FROM Invoices " +
                    "WHERE InvoiceNumber = ?;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the invoice header SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that loads all line items for one invoice.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetInvoiceLinesByNumberSql()
        {
            try
            {
                return
                    "SELECT InvoiceNumber, ItemCode, ItemDescription, ItemCost " +
                    "FROM InvoiceItems " +
                    "WHERE InvoiceNumber = ? " +
                    "ORDER BY InvoiceItemId;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the invoice line SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that returns the latest invoice number.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetLatestInvoiceNumberSql()
        {
            try
            {
                return
                    "SELECT MAX(InvoiceNumber) AS MaxInvoiceNumber " +
                    "FROM Invoices;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the max invoice number SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that will eventually insert a new invoice header.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetInsertInvoiceSql()
        {
            try
            {
                return
                    "INSERT INTO Invoices (InvoiceDate, TotalCharge) " +
                    "VALUES (?, ?);";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the insert invoice SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that will eventually insert a new invoice line.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetInsertInvoiceLineSql()
        {
            try
            {
                return
                    "INSERT INTO InvoiceItems (InvoiceNumber, ItemCode, ItemDescription, ItemCost) " +
                    "VALUES (?, ?, ?, ?);";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the insert invoice line SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that updates an existing invoice header.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetUpdateInvoiceSql()
        {
            try
            {
                return
                    "UPDATE Invoices " +
                    "SET InvoiceDate = ?, TotalCharge = ? " +
                    "WHERE InvoiceNumber = ?;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the update invoice SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that deletes all line items for an invoice.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetDeleteInvoiceLinesByNumberSql()
        {
            try
            {
                return
                    "DELETE FROM InvoiceItems " +
                    "WHERE InvoiceNumber = ?;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the delete invoice lines SQL statement.", ex);
            }
        }
    }
}
