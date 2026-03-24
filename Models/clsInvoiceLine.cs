namespace Group_Assignment.Models
{
    /// <summary>
    /// Represents one line item displayed on an invoice.
    /// </summary>
    public sealed class clsInvoiceLine
    {
        /// <summary>
        /// Gets or sets the display line number.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        public string ItemCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        public string ItemDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the item cost.
        /// </summary>
        public decimal ItemCost { get; set; }
    }
}
