using System.Data.OleDb;
using Group_Assignment.Data;
using Group_Assignment.Models;

namespace Group_Assignment.Main
{
    /// <summary>
    /// Contains business logic used by the main invoice window.
    /// </summary>
    public sealed class clsMainLogic
    {
        private readonly clsMainSQL _mainSql;
        private readonly clsDatabaseConnection _databaseConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="clsMainLogic"/> class.
        /// </summary>
        /// <param name="mainSql">The SQL provider for the main window.</param>
        /// <param name="databaseConnection">The shared database connection helper.</param>
        public clsMainLogic(clsMainSQL mainSql, clsDatabaseConnection databaseConnection)
        {
            try
            {
                _mainSql = mainSql;
                _databaseConnection = databaseConnection;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to initialize the main logic layer.", ex);
            }
        }

        /// <summary>
        /// Gets the placeholder invoice number used for unsaved invoices.
        /// </summary>
        public string NewInvoiceNumberPlaceholder
        {
            get
            {
                try
                {
                    return "TBD";
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to get the placeholder invoice number.", ex);
                }
            }
        }

        /// <summary>
        /// Gets the shared database connection helper.
        /// </summary>
        public clsDatabaseConnection DatabaseConnection
        {
            get
            {
                try
                {
                    return _databaseConnection;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to get the database connection helper.", ex);
                }
            }
        }

        /// <summary>
        /// Gets the current database connection status.
        /// </summary>
        /// <returns>A tuple describing the database availability.</returns>
        public async Task<(bool Success, string Message)> GetDatabaseStatusAsync()
        {
            try
            {
                return await _databaseConnection.TestConnectionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to get the database status.", ex);
            }
        }

        /// <summary>
        /// Loads the item definitions shown in the main window item drop-down.
        /// </summary>
        /// <returns>A list of available items.</returns>
        public async Task<IReadOnlyList<clsItemDefinition>> LoadItemDefinitionsAsync()
        {
            try
            {
                List<clsItemDefinition> items = new();

                if (!_databaseConnection.DatabaseFileExists())
                {
                    return items;
                }

                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                using OleDbCommand command = new(_mainSql.GetAllItemDefinitionsSql(), connection);

                await connection.OpenAsync().ConfigureAwait(false);
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader is null)
                {
                    return items;
                }

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    items.Add(new clsItemDefinition
                    {
                        ItemCode = Convert.ToString(reader["ItemCode"]) ?? string.Empty,
                        ItemDescription = Convert.ToString(reader["ItemDescription"]) ?? string.Empty,
                        ItemCost = Convert.ToDecimal(reader["ItemCost"]),
                    });
                }

                return items;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load the item definitions.", ex);
            }
        }

        /// <summary>
        /// Loads one invoice header for display on the main window.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number to load.</param>
        /// <returns>The invoice header when found; otherwise null.</returns>
        public async Task<clsInvoiceSummary?> LoadInvoiceHeaderAsync(int invoiceNumber)
        {
            try
            {
                if (!_databaseConnection.DatabaseFileExists())
                {
                    return null;
                }

                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                using OleDbCommand command = new(_mainSql.GetInvoiceHeaderByNumberSql(), connection);
                command.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceNumber;

                await connection.OpenAsync().ConfigureAwait(false);
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader is null || !await reader.ReadAsync().ConfigureAwait(false))
                {
                    return null;
                }

                return new clsInvoiceSummary
                {
                    InvoiceNumber = Convert.ToInt32(reader["InvoiceNumber"]),
                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                    TotalCharge = Convert.ToDecimal(reader["TotalCharge"]),
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to load invoice {invoiceNumber}.", ex);
            }
        }

        /// <summary>
        /// Loads the invoice line items for the requested invoice number.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number to load.</param>
        /// <returns>The collection of invoice line items.</returns>
        public async Task<IReadOnlyList<clsInvoiceLine>> LoadInvoiceLinesAsync(int invoiceNumber)
        {
            try
            {
                List<clsInvoiceLine> invoiceLines = new();

                if (!_databaseConnection.DatabaseFileExists())
                {
                    return invoiceLines;
                }

                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                using OleDbCommand command = new(_mainSql.GetInvoiceLinesByNumberSql(), connection);
                command.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceNumber;

                await connection.OpenAsync().ConfigureAwait(false);
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader is null)
                {
                    return invoiceLines;
                }

                int lineNumber = 1;

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    invoiceLines.Add(new clsInvoiceLine
                    {
                        LineNumber = lineNumber,
                        ItemCode = Convert.ToString(reader["ItemCode"]) ?? string.Empty,
                        ItemDescription = Convert.ToString(reader["ItemDescription"]) ?? string.Empty,
                        ItemCost = Convert.ToDecimal(reader["ItemCost"]),
                    });

                    lineNumber++;
                }

                return invoiceLines;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to load invoice items for invoice {invoiceNumber}.", ex);
            }
        }

        /// <summary>
        /// Calculates the displayed total for the current invoice line collection.
        /// </summary>
        /// <param name="invoiceLines">The invoice lines to total.</param>
        /// <returns>The calculated invoice total.</returns>
        public decimal CalculateInvoiceTotal(IEnumerable<clsInvoiceLine> invoiceLines)
        {
            try
            {
                return invoiceLines.Sum(line => line.ItemCost);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to calculate the invoice total.", ex);
            }
        }

        /// <summary>
        /// Saves a new invoice or updates an existing invoice.
        /// </summary>
        /// <param name="invoiceDate">The invoice date selected by the user.</param>
        /// <param name="invoiceLines">The invoice line items to save.</param>
        /// <param name="invoiceNumber">The invoice number when updating an existing invoice.</param>
        /// <returns>The saved invoice number.</returns>
        public async Task<int> SaveInvoiceAsync(
            DateTime invoiceDate,
            IEnumerable<clsInvoiceLine> invoiceLines,
            int? invoiceNumber = null)
        {
            try
            {
                List<clsInvoiceLine> invoiceLineList = ValidateAndCopyInvoiceLines(invoiceLines);
                decimal totalCharge = CalculateInvoiceTotal(invoiceLineList);
                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                await connection.OpenAsync().ConfigureAwait(false);

                using OleDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    int savedInvoiceNumber = invoiceNumber.HasValue
                        ? UpdateExistingInvoice(connection, transaction, invoiceNumber.Value, invoiceDate, totalCharge, invoiceLineList)
                        : InsertNewInvoice(connection, transaction, invoiceDate, totalCharge, invoiceLineList);

                    transaction.Commit();
                    return savedInvoiceNumber;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to save the invoice.", ex);
            }
        }

        /// <summary>
        /// Validates the invoice line items and creates a detached list for saving.
        /// </summary>
        /// <param name="invoiceLines">The source invoice line items.</param>
        /// <returns>A safe list of invoice lines to persist.</returns>
        private static List<clsInvoiceLine> ValidateAndCopyInvoiceLines(IEnumerable<clsInvoiceLine> invoiceLines)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(invoiceLines);

                List<clsInvoiceLine> copiedLines = invoiceLines
                    .Select(line => new clsInvoiceLine
                    {
                        LineNumber = line.LineNumber,
                        ItemCode = line.ItemCode,
                        ItemDescription = line.ItemDescription,
                        ItemCost = line.ItemCost,
                    })
                    .ToList();

                if (copiedLines.Count == 0)
                {
                    throw new ApplicationException("At least one invoice item is required before saving.");
                }

                return copiedLines;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to validate the invoice line items.", ex);
            }
        }

        /// <summary>
        /// Inserts a new invoice header and lines inside the current transaction.
        /// </summary>
        /// <param name="connection">The open database connection.</param>
        /// <param name="transaction">The active database transaction.</param>
        /// <param name="invoiceDate">The invoice date to store.</param>
        /// <param name="totalCharge">The invoice total charge.</param>
        /// <param name="invoiceLines">The invoice lines to insert.</param>
        /// <returns>The inserted invoice number.</returns>
        private int InsertNewInvoice(
            OleDbConnection connection,
            OleDbTransaction transaction,
            DateTime invoiceDate,
            decimal totalCharge,
            IReadOnlyList<clsInvoiceLine> invoiceLines)
        {
            try
            {
                using OleDbCommand insertInvoiceCommand = new(_mainSql.GetInsertInvoiceSql(), connection, transaction);
                insertInvoiceCommand.Parameters.Add("@InvoiceDate", OleDbType.DBDate).Value = invoiceDate.Date;
                insertInvoiceCommand.Parameters.Add("@TotalCharge", OleDbType.Currency).Value = totalCharge;
                insertInvoiceCommand.ExecuteNonQuery();

                using OleDbCommand maxInvoiceNumberCommand = new(_mainSql.GetLatestInvoiceNumberSql(), connection, transaction);
                object? maxInvoiceNumber = maxInvoiceNumberCommand.ExecuteScalar();

                if (maxInvoiceNumber is null || maxInvoiceNumber == DBNull.Value)
                {
                    throw new ApplicationException("The invoice number could not be determined after insert.");
                }

                int savedInvoiceNumber = Convert.ToInt32(maxInvoiceNumber);
                InsertInvoiceLines(connection, transaction, savedInvoiceNumber, invoiceLines);
                return savedInvoiceNumber;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to insert the new invoice.", ex);
            }
        }

        /// <summary>
        /// Updates an existing invoice header and replaces its lines inside the current transaction.
        /// </summary>
        /// <param name="connection">The open database connection.</param>
        /// <param name="transaction">The active database transaction.</param>
        /// <param name="invoiceNumber">The invoice number being updated.</param>
        /// <param name="invoiceDate">The invoice date to store.</param>
        /// <param name="totalCharge">The invoice total charge.</param>
        /// <param name="invoiceLines">The invoice lines to persist.</param>
        /// <returns>The updated invoice number.</returns>
        private int UpdateExistingInvoice(
            OleDbConnection connection,
            OleDbTransaction transaction,
            int invoiceNumber,
            DateTime invoiceDate,
            decimal totalCharge,
            IReadOnlyList<clsInvoiceLine> invoiceLines)
        {
            try
            {
                using OleDbCommand updateInvoiceCommand = new(_mainSql.GetUpdateInvoiceSql(), connection, transaction);
                updateInvoiceCommand.Parameters.Add("@InvoiceDate", OleDbType.DBDate).Value = invoiceDate.Date;
                updateInvoiceCommand.Parameters.Add("@TotalCharge", OleDbType.Currency).Value = totalCharge;
                updateInvoiceCommand.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceNumber;
                updateInvoiceCommand.ExecuteNonQuery();

                using OleDbCommand deleteInvoiceLinesCommand = new(_mainSql.GetDeleteInvoiceLinesByNumberSql(), connection, transaction);
                deleteInvoiceLinesCommand.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceNumber;
                deleteInvoiceLinesCommand.ExecuteNonQuery();

                InsertInvoiceLines(connection, transaction, invoiceNumber, invoiceLines);
                return invoiceNumber;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to update invoice {invoiceNumber}.", ex);
            }
        }

        /// <summary>
        /// Inserts all line items for the requested invoice number.
        /// </summary>
        /// <param name="connection">The open database connection.</param>
        /// <param name="transaction">The active database transaction.</param>
        /// <param name="invoiceNumber">The invoice number to associate with the lines.</param>
        /// <param name="invoiceLines">The line items to insert.</param>
        private void InsertInvoiceLines(
            OleDbConnection connection,
            OleDbTransaction transaction,
            int invoiceNumber,
            IEnumerable<clsInvoiceLine> invoiceLines)
        {
            try
            {
                foreach (clsInvoiceLine invoiceLine in invoiceLines)
                {
                    using OleDbCommand insertLineCommand = new(_mainSql.GetInsertInvoiceLineSql(), connection, transaction);
                    insertLineCommand.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceNumber;
                    insertLineCommand.Parameters.Add("@ItemCode", OleDbType.VarWChar, 20).Value = invoiceLine.ItemCode;
                    insertLineCommand.Parameters.Add("@ItemDescription", OleDbType.VarWChar, 100).Value = invoiceLine.ItemDescription;
                    insertLineCommand.Parameters.Add("@ItemCost", OleDbType.Currency).Value = invoiceLine.ItemCost;
                    insertLineCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to insert invoice line items for invoice {invoiceNumber}.", ex);
            }
        }
    }
}
