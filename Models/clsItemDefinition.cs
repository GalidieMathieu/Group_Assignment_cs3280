namespace Group_Assignment.Models
{
    /// <summary>
    /// Represents an item definition loaded from the Access item table.
    /// </summary>
    public sealed class clsItemDefinition
    {
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

        /// <summary>
        /// Gets the display text shown in the item combo box.
        /// </summary>
        public string DisplayText
        {
            get
            {
                try
                {
                    return $"{ItemCode} - {ItemDescription}";
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to build the item display text.", ex);
                }
            }
        }
    }
}
