namespace Group_Assignment.Models
{
    /// <summary>
    /// Represents an invoice summary row used by the main and search windows.
    /// </summary>
    public sealed class clsInvoiceSummary
    {
        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the total charge for the invoice.
        /// </summary>
        public decimal TotalCharge { get; set; }
    }
}
