using System.Collections.ObjectModel;
using System.Windows;
using Group_Assignment.Data;
using Group_Assignment.Models;
using Group_Assignment.Search;

namespace Group_Assignment.Main
{
    /// <summary>
    /// Interaction logic for the main invoice window.
    /// </summary>
    public partial class wndMain : Window
    {
        private readonly clsMainLogic _mainLogic;
        private readonly ObservableCollection<clsItemDefinition> _itemDefinitions;
        private readonly ObservableCollection<clsInvoiceLine> _invoiceLines;
        private string _databaseStatusMessage = "checking...";
        private int? _currentInvoiceNumber;
        private bool _isEditMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="wndMain"/> class.
        /// </summary>
        public wndMain()
            : this(new clsMainLogic(new clsMainSQL(), new clsDatabaseConnection()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="wndMain"/> class with injected logic.
        /// </summary>
        /// <param name="mainLogic">The main business logic layer.</param>
        public wndMain(clsMainLogic mainLogic)
        {
            try
            {
                InitializeComponent();
                _mainLogic = mainLogic;
                _itemDefinitions = new ObservableCollection<clsItemDefinition>();
                _invoiceLines = new ObservableCollection<clsInvoiceLine>();

                cmbItems.ItemsSource = _itemDefinitions;
                dgInvoiceItems.ItemsSource = _invoiceLines;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to initialize the main window.", ex);
            }
        }

        /// <summary>
        /// Handles the main window loaded event.
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshDatabaseStatusAsync();
                await RefreshItemDefinitionsAsync();
                PrepareNewInvoice();
            }
            catch (Exception ex)
            {
                ShowError("The main window could not finish loading.", ex);
            }
        }

        /// <summary>
        /// Starts a new invoice from the menu.
        /// </summary>
        private void mnuNewInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrepareNewInvoice();
            }
            catch (Exception ex)
            {
                ShowError("Unable to prepare a new invoice.", ex);
            }
        }

        /// <summary>
        /// Places the current invoice into edit mode from the menu.
        /// </summary>
        private void mnuEditInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetEditMode(true);
            }
            catch (Exception ex)
            {
                ShowError("Unable to switch the invoice into edit mode.", ex);
            }
        }

        /// <summary>
        /// Opens the search window and loads the selected invoice when one is chosen.
        /// </summary>
        private async void mnuSearchInvoices_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wndSearch searchWindow = new(new clsSearchLogic(new clsSearchSQL(), _mainLogic.DatabaseConnection))
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };

                bool? dialogResult = searchWindow.ShowDialog();

                if (dialogResult == true && searchWindow.SelectedInvoiceNumber.HasValue)
                {
                    await LoadInvoiceAsync(searchWindow.SelectedInvoiceNumber.Value);
                }
            }
            catch (Exception ex)
            {
                ShowError("Unable to open the search window.", ex);
            }
        }

        /// <summary>
        /// Handles the placeholder manage items menu click.
        /// </summary>
        private void mnuManageItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateStatusBar("Items management is outside the current two-window build.");
            }
            catch (Exception ex)
            {
                ShowError("Unable to open the items window placeholder.", ex);
            }
        }

        /// <summary>
        /// Starts a new invoice from the button.
        /// </summary>
        private void btnNewInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrepareNewInvoice();
            }
            catch (Exception ex)
            {
                ShowError("Unable to prepare a new invoice.", ex);
            }
        }

        /// <summary>
        /// Places the current invoice into edit mode.
        /// </summary>
        private void btnEditInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetEditMode(true);
            }
            catch (Exception ex)
            {
                ShowError("Unable to switch the invoice into edit mode.", ex);
            }
        }

        /// <summary>
        /// Handles the placeholder save button click.
        /// </summary>
        private void btnSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _ = SaveCurrentInvoiceAsync();
            }
            catch (Exception ex)
            {
                ShowError("Unable to save the current invoice.", ex);
            }
        }

        /// <summary>
        /// Updates the displayed item cost when the selected item changes.
        /// </summary>
        private void cmbItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbItems.SelectedItem is clsItemDefinition selectedItem)
                {
                    txtItemCost.Text = selectedItem.ItemCost.ToString("C");
                }
                else
                {
                    txtItemCost.Text = 0m.ToString("C");
                }

                btnAddItem.IsEnabled = _isEditMode && cmbItems.SelectedItem is clsItemDefinition;
            }
            catch (Exception ex)
            {
                ShowError("Unable to update the selected item information.", ex);
            }
        }

        /// <summary>
        /// Adds the selected item to the invoice line grid.
        /// </summary>
        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isEditMode)
                {
                    MessageBox.Show(
                        "Switch the invoice into edit mode before adding items.",
                        "Read Only Invoice",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (cmbItems.SelectedItem is not clsItemDefinition selectedItem)
                {
                    MessageBox.Show(
                        "Select an item before trying to add it.",
                        "Missing Item",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _invoiceLines.Add(new clsInvoiceLine
                {
                    LineNumber = _invoiceLines.Count + 1,
                    ItemCode = selectedItem.ItemCode,
                    ItemDescription = selectedItem.ItemDescription,
                    ItemCost = selectedItem.ItemCost,
                });

                UpdateDisplayedTotal();
            }
            catch (Exception ex)
            {
                ShowError("Unable to add the selected item to the invoice.", ex);
            }
        }

        /// <summary>
        /// Removes the selected invoice line from the grid.
        /// </summary>
        private void btnRemoveSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_isEditMode)
                {
                    MessageBox.Show(
                        "Switch the invoice into edit mode before removing items.",
                        "Read Only Invoice",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                if (dgInvoiceItems.SelectedItem is not clsInvoiceLine selectedLine)
                {
                    MessageBox.Show(
                        "Select an invoice line before trying to remove it.",
                        "Missing Line Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                _invoiceLines.Remove(selectedLine);
                RenumberInvoiceLines();
                UpdateDisplayedTotal();
            }
            catch (Exception ex)
            {
                ShowError("Unable to remove the selected invoice item.", ex);
            }
        }

        /// <summary>
        /// Updates button state when the selected invoice line changes.
        /// </summary>
        private void dgInvoiceItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                btnRemoveSelectedItem.IsEnabled = _isEditMode && dgInvoiceItems.SelectedItem is clsInvoiceLine;
            }
            catch (Exception ex)
            {
                ShowError("Unable to update the invoice item selection state.", ex);
            }
        }

        /// <summary>
        /// Loads the current list of item definitions into the item drop-down.
        /// </summary>
        private async Task RefreshItemDefinitionsAsync()
        {
            try
            {
                IReadOnlyList<clsItemDefinition> items = await _mainLogic.LoadItemDefinitionsAsync();

                _itemDefinitions.Clear();

                foreach (clsItemDefinition item in items)
                {
                    _itemDefinitions.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to refresh the item drop-down.", ex);
            }
        }

        /// <summary>
        /// Refreshes the connection status text shown at the bottom of the main window.
        /// </summary>
        private async Task RefreshDatabaseStatusAsync()
        {
            try
            {
                (bool _, string message) = await _mainLogic.GetDatabaseStatusAsync();
                _databaseStatusMessage = message;
                UpdateStatusBar();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to refresh the database status.", ex);
            }
        }

        /// <summary>
        /// Resets the main window for a new invoice entry.
        /// </summary>
        private void PrepareNewInvoice()
        {
            try
            {
                _currentInvoiceNumber = null;
                txtInvoiceNumber.Text = _mainLogic.NewInvoiceNumberPlaceholder;
                dpInvoiceDate.SelectedDate = null;
                cmbItems.SelectedIndex = -1;
                txtItemCost.Text = 0m.ToString("C");
                _invoiceLines.Clear();
                UpdateDisplayedTotal();
                SetEditMode(true);
                UpdateStatusBar("Ready for a new invoice.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to prepare the new invoice screen.", ex);
            }
        }

        /// <summary>
        /// Loads an existing invoice into the main window.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number to load.</param>
        private async Task LoadInvoiceAsync(int invoiceNumber)
        {
            try
            {
                clsInvoiceSummary? invoiceHeader = await _mainLogic.LoadInvoiceHeaderAsync(invoiceNumber);

                if (invoiceHeader is null)
                {
                    MessageBox.Show(
                        $"Invoice {invoiceNumber} was not found in the current database.",
                        "Invoice Not Found",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                IReadOnlyList<clsInvoiceLine> invoiceLines = await _mainLogic.LoadInvoiceLinesAsync(invoiceNumber);

                _currentInvoiceNumber = invoiceHeader.InvoiceNumber;
                txtInvoiceNumber.Text = invoiceHeader.InvoiceNumber.ToString();
                dpInvoiceDate.SelectedDate = invoiceHeader.InvoiceDate;

                _invoiceLines.Clear();

                foreach (clsInvoiceLine invoiceLine in invoiceLines)
                {
                    _invoiceLines.Add(invoiceLine);
                }

                UpdateDisplayedTotal();
                SetEditMode(false);
                UpdateStatusBar($"Loaded invoice {invoiceNumber} in read-only mode.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Unable to load invoice {invoiceNumber}.", ex);
            }
        }

        /// <summary>
        /// Applies read-only or edit mode to the main invoice form.
        /// </summary>
        /// <param name="isEditMode">True when the form should be editable.</param>
        private void SetEditMode(bool isEditMode)
        {
            try
            {
                _isEditMode = isEditMode;
                dpInvoiceDate.IsEnabled = isEditMode;
                cmbItems.IsEnabled = isEditMode;
                btnAddItem.IsEnabled = isEditMode && cmbItems.SelectedItem is clsItemDefinition;
                btnRemoveSelectedItem.IsEnabled = isEditMode && dgInvoiceItems.SelectedItem is clsInvoiceLine;
                btnEditInvoice.IsEnabled = !isEditMode && _currentInvoiceNumber.HasValue;
                btnSaveInvoice.IsEnabled = isEditMode;
                mnuManageItems.IsEnabled = !isEditMode;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to update the form mode.", ex);
            }
        }

        /// <summary>
        /// Updates the displayed invoice total.
        /// </summary>
        private void UpdateDisplayedTotal()
        {
            try
            {
                decimal total = _mainLogic.CalculateInvoiceTotal(_invoiceLines);
                txtTotalCharge.Text = total.ToString("C");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to update the invoice total.", ex);
            }
        }

        /// <summary>
        /// Renumbers invoice line rows after a delete.
        /// </summary>
        private void RenumberInvoiceLines()
        {
            try
            {
                for (int lineIndex = 0; lineIndex < _invoiceLines.Count; lineIndex++)
                {
                    _invoiceLines[lineIndex].LineNumber = lineIndex + 1;
                }

                dgInvoiceItems.Items.Refresh();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to renumber the invoice lines.", ex);
            }
        }

        /// <summary>
        /// Validates and saves the current invoice.
        /// </summary>
        private async Task SaveCurrentInvoiceAsync()
        {
            try
            {
                if (!TryGetInvoiceDateForSave(out DateTime invoiceDate))
                {
                    return;
                }

                if (_invoiceLines.Count == 0)
                {
                    MessageBox.Show(
                        "Add at least one invoice item before saving.",
                        "Missing Invoice Items",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                int savedInvoiceNumber = await _mainLogic.SaveInvoiceAsync(invoiceDate, _invoiceLines, _currentInvoiceNumber);
                await LoadInvoiceAsync(savedInvoiceNumber);
                UpdateStatusBar($"Invoice {savedInvoiceNumber} saved successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Unable to save the current invoice.", ex);
            }
        }

        /// <summary>
        /// Validates the current invoice date before save.
        /// </summary>
        /// <param name="invoiceDate">The selected invoice date when valid.</param>
        /// <returns>True when the invoice date is valid.</returns>
        private bool TryGetInvoiceDateForSave(out DateTime invoiceDate)
        {
            try
            {
                if (!dpInvoiceDate.SelectedDate.HasValue)
                {
                    invoiceDate = default;

                    MessageBox.Show(
                        "Select an invoice date before saving.",
                        "Missing Invoice Date",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return false;
                }

                invoiceDate = dpInvoiceDate.SelectedDate.Value.Date;
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to validate the invoice date.", ex);
            }
        }

        /// <summary>
        /// Updates the footer status text with the current database message and optional activity text.
        /// </summary>
        /// <param name="activityMessage">The optional activity message.</param>
        private void UpdateStatusBar(string? activityMessage = null)
        {
            try
            {
                txtDatabaseStatus.Text = string.IsNullOrWhiteSpace(activityMessage)
                    ? $"Database status: {_databaseStatusMessage}"
                    : $"Database status: {_databaseStatusMessage} | {activityMessage}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to update the status bar.", ex);
            }
        }

        /// <summary>
        /// Shows a user-friendly error dialog.
        /// </summary>
        /// <param name="message">The context-specific message.</param>
        /// <param name="exception">The exception that occurred.</param>
        private void ShowError(string message, Exception exception)
        {
            try
            {
                MessageBox.Show(
                    $"{message}{Environment.NewLine}{Environment.NewLine}{exception.Message}",
                    "Application Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch
            {
            }
        }
    }
}
