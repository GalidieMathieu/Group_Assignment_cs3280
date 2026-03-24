using System.Data.OleDb;
using Group_Assignment.Data;
using Group_Assignment.Models;

namespace Group_Assignment.Search
{
    /// <summary>
    /// Contains business logic used by the invoice search window.
    /// </summary>
    public sealed class clsSearchLogic
    {
        private readonly clsSearchSQL _searchSql;
        private readonly clsDatabaseConnection _databaseConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="clsSearchLogic"/> class.
        /// </summary>
        /// <param name="searchSql">The search SQL provider.</param>
        /// <param name="databaseConnection">The shared database connection helper.</param>
        public clsSearchLogic(clsSearchSQL searchSql, clsDatabaseConnection databaseConnection)
        {
            try
            {
                _searchSql = searchSql;
                _databaseConnection = databaseConnection;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to initialize the search logic layer.", ex);
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
                throw new ApplicationException("Unable to get the search database status.", ex);
            }
        }

        /// <summary>
        /// Loads the distinct invoice numbers used by the search filter.
        /// </summary>
        /// <returns>The list of invoice numbers.</returns>
        public async Task<IReadOnlyList<int>> LoadInvoiceNumbersAsync()
        {
            try
            {
                List<int> invoiceNumbers = new();

                if (!_databaseConnection.DatabaseFileExists())
                {
                    return invoiceNumbers;
                }

                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                using OleDbCommand command = new(_searchSql.GetDistinctInvoiceNumbersSql(), connection);

                await connection.OpenAsync().ConfigureAwait(false);
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader is null)
                {
                    return invoiceNumbers;
                }

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    invoiceNumbers.Add(Convert.ToInt32(reader["InvoiceNumber"]));
                }

                return invoiceNumbers;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load the invoice number filter values.", ex);
            }
        }

        /// <summary>
        /// Loads the distinct total charges used by the search filter.
        /// </summary>
        /// <returns>The list of unique total charges.</returns>
        public async Task<IReadOnlyList<decimal>> LoadTotalChargesAsync()
        {
            try
            {
                List<decimal> totalCharges = new();

                if (!_databaseConnection.DatabaseFileExists())
                {
                    return totalCharges;
                }

                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                using OleDbCommand command = new(_searchSql.GetDistinctTotalChargesSql(), connection);

                await connection.OpenAsync().ConfigureAwait(false);
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader is null)
                {
                    return totalCharges;
                }

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    totalCharges.Add(Convert.ToDecimal(reader["TotalCharge"]));
                }

                return totalCharges;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load the total charge filter values.", ex);
            }
        }

        /// <summary>
        /// Loads invoice summaries using the selected filters.
        /// </summary>
        /// <param name="invoiceNumber">The optional invoice number filter.</param>
        /// <param name="invoiceDate">The optional invoice date filter.</param>
        /// <param name="totalCharge">The optional total charge filter.</param>
        /// <returns>The filtered invoice list.</returns>
        public async Task<IReadOnlyList<clsInvoiceSummary>> LoadInvoicesAsync(
            int? invoiceNumber = null,
            DateTime? invoiceDate = null,
            decimal? totalCharge = null)
        {
            try
            {
                List<clsInvoiceSummary> invoices = new();

                if (!_databaseConnection.DatabaseFileExists())
                {
                    return invoices;
                }

                string providerName = await _databaseConnection.ResolveProviderNameAsync().ConfigureAwait(false);
                string invoiceSearchSql = _searchSql.BuildInvoiceSearchSql(
                    invoiceNumber.HasValue,
                    invoiceDate.HasValue,
                    totalCharge.HasValue);

                using OleDbConnection connection = _databaseConnection.CreateConnection(providerName);
                using OleDbCommand command = new(invoiceSearchSql, connection);

                AddSearchParameters(command, invoiceNumber, invoiceDate, totalCharge);

                await connection.OpenAsync().ConfigureAwait(false);
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                if (reader is null)
                {
                    return invoices;
                }

                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    invoices.Add(new clsInvoiceSummary
                    {
                        InvoiceNumber = Convert.ToInt32(reader["InvoiceNumber"]),
                        InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                        TotalCharge = Convert.ToDecimal(reader["TotalCharge"]),
                    });
                }

                return invoices;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load invoices for the search window.", ex);
            }
        }

        /// <summary>
        /// Adds search parameters to the command in the same order used by the SQL builder.
        /// </summary>
        /// <param name="command">The command receiving the parameters.</param>
        /// <param name="invoiceNumber">The optional invoice number filter.</param>
        /// <param name="invoiceDate">The optional invoice date filter.</param>
        /// <param name="totalCharge">The optional total charge filter.</param>
        private void AddSearchParameters(
            OleDbCommand command,
            int? invoiceNumber,
            DateTime? invoiceDate,
            decimal? totalCharge)
        {
            try
            {
                if (invoiceNumber.HasValue)
                {
                    command.Parameters.Add("@InvoiceNumber", OleDbType.Integer).Value = invoiceNumber.Value;
                }

                if (invoiceDate.HasValue)
                {
                    command.Parameters.Add("@InvoiceDate", OleDbType.DBDate).Value = invoiceDate.Value.Date;
                }

                if (totalCharge.HasValue)
                {
                    command.Parameters.Add("@TotalCharge", OleDbType.Currency).Value = totalCharge.Value;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to add the search filter parameters.", ex);
            }
        }
    }
}
