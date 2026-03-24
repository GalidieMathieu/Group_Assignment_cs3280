using System.Text;

namespace Group_Assignment.Search
{
    /// <summary>
    /// Stores SQL statements used by the search window.
    /// </summary>
    public sealed class clsSearchSQL
    {
        /// <summary>
        /// Gets the SQL that loads all invoice summaries.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetAllInvoicesSql()
        {
            try
            {
                return
                    "SELECT InvoiceNumber, InvoiceDate, TotalCharge " +
                    "FROM Invoices " +
                    "ORDER BY InvoiceNumber DESC;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the all invoices SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that loads distinct invoice numbers for the search filter.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetDistinctInvoiceNumbersSql()
        {
            try
            {
                return
                    "SELECT DISTINCT InvoiceNumber " +
                    "FROM Invoices " +
                    "ORDER BY InvoiceNumber;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the invoice number filter SQL statement.", ex);
            }
        }

        /// <summary>
        /// Gets the SQL that loads distinct invoice totals for the search filter.
        /// </summary>
        /// <returns>The Access SQL statement.</returns>
        public string GetDistinctTotalChargesSql()
        {
            try
            {
                return
                    "SELECT DISTINCT TotalCharge " +
                    "FROM Invoices " +
                    "ORDER BY TotalCharge;";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the total charge filter SQL statement.", ex);
            }
        }

        /// <summary>
        /// Builds the SQL used to search invoices with optional filters.
        /// </summary>
        /// <param name="hasInvoiceNumber">True when the invoice number filter is selected.</param>
        /// <param name="hasInvoiceDate">True when the invoice date filter is selected.</param>
        /// <param name="hasTotalCharge">True when the total charge filter is selected.</param>
        /// <returns>The Access SQL statement.</returns>
        public string BuildInvoiceSearchSql(bool hasInvoiceNumber, bool hasInvoiceDate, bool hasTotalCharge)
        {
            try
            {
                StringBuilder sqlBuilder = new();
                List<string> whereClauses = new();

                sqlBuilder.Append("SELECT InvoiceNumber, InvoiceDate, TotalCharge FROM Invoices");

                if (hasInvoiceNumber)
                {
                    whereClauses.Add("InvoiceNumber = ?");
                }

                if (hasInvoiceDate)
                {
                    whereClauses.Add("DateValue(InvoiceDate) = ?");
                }

                if (hasTotalCharge)
                {
                    whereClauses.Add("TotalCharge = ?");
                }

                if (whereClauses.Count > 0)
                {
                    sqlBuilder.Append(" WHERE ");
                    sqlBuilder.Append(string.Join(" AND ", whereClauses));
                }

                sqlBuilder.Append(" ORDER BY InvoiceNumber DESC;");

                return sqlBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the filtered invoice search SQL statement.", ex);
            }
        }
    }
}
