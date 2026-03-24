using System.Collections.ObjectModel;
using System.Windows;
using Group_Assignment.Data;
using Group_Assignment.Models;

namespace Group_Assignment.Search
{
    /// <summary>
    /// Interaction logic for the invoice search window.
    /// </summary>
    public partial class wndSearch : Window
    {
        private readonly clsSearchLogic _searchLogic;
        private readonly ObservableCollection<clsInvoiceSummary> _invoiceSummaries;
        private readonly ObservableCollection<int> _invoiceNumbers;
        private readonly ObservableCollection<decimal> _totalCharges;
        private bool _isRefreshing;

        /// <summary>
        /// Initializes a new instance of the <see cref="wndSearch"/> class.
        /// </summary>
        public wndSearch()
            : this(new clsSearchLogic(new clsSearchSQL(), new clsDatabaseConnection()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="wndSearch"/> class with injected logic.
        /// </summary>
        /// <param name="searchLogic">The search business logic layer.</param>
        public wndSearch(clsSearchLogic searchLogic)
        {
            try
            {
                InitializeComponent();
                _searchLogic = searchLogic;
                _invoiceSummaries = new ObservableCollection<clsInvoiceSummary>();
                _invoiceNumbers = new ObservableCollection<int>();
                _totalCharges = new ObservableCollection<decimal>();

                dgInvoices.ItemsSource = _invoiceSummaries;
                cmbInvoiceNumber.ItemsSource = _invoiceNumbers;
                cmbTotalCharge.ItemsSource = _totalCharges;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to initialize the search window.", ex);
            }
        }

        /// <summary>
        /// Gets the invoice number selected by the user.
        /// </summary>
        public int? SelectedInvoiceNumber { get; private set; }

        /// <summary>
        /// Handles the search window loaded event.
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshSearchScreenAsync();
            }
            catch (Exception ex)
            {
                ShowError("The search window could not finish loading.", ex);
            }
        }

        /// <summary>
        /// Applies the selected filters whenever one of the filter controls changes.
        /// </summary>
        private async void SearchFilter_Changed(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (_isRefreshing)
                {
                    return;
                }

                await LoadInvoicesAsync();
            }
            catch (Exception ex)
            {
                ShowError("Unable to apply the selected search filters.", ex);
            }
        }

        /// <summary>
        /// Clears the current filter selections and reloads the search grid.
        /// </summary>
        private async void btnClearSelection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isRefreshing = true;
                cmbInvoiceNumber.SelectedIndex = -1;
                cmbTotalCharge.SelectedIndex = -1;
                dpInvoiceDate.SelectedDate = null;
                _isRefreshing = false;

                await LoadInvoicesAsync();
            }
            catch (Exception ex)
            {
                ShowError("Unable to clear the selected search filters.", ex);
            }
        }

        /// <summary>
        /// Returns the selected invoice number to the main window.
        /// </summary>
        private void btnSelectInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgInvoices.SelectedItem is not clsInvoiceSummary selectedInvoice)
                {
                    MessageBox.Show(
                        "Select an invoice from the list before continuing.",
                        "Missing Invoice Selection",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                SelectedInvoiceNumber = selectedInvoice.InvoiceNumber;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                ShowError("Unable to select the requested invoice.", ex);
            }
        }

        /// <summary>
        /// Closes the search window without returning a selection.
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                ShowError("Unable to close the search window.", ex);
            }
        }

        /// <summary>
        /// Loads the search filters and invoice list.
        /// </summary>
        private async Task RefreshSearchScreenAsync()
        {
            try
            {
                _isRefreshing = true;

                IReadOnlyList<int> invoiceNumbers = await _searchLogic.LoadInvoiceNumbersAsync();
                IReadOnlyList<decimal> totalCharges = await _searchLogic.LoadTotalChargesAsync();

                _invoiceNumbers.Clear();
                foreach (int invoiceNumber in invoiceNumbers)
                {
                    _invoiceNumbers.Add(invoiceNumber);
                }

                _totalCharges.Clear();
                foreach (decimal totalCharge in totalCharges)
                {
                    _totalCharges.Add(totalCharge);
                }

                _isRefreshing = false;

                await LoadInvoicesAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to refresh the search screen.", ex);
            }
        }

        /// <summary>
        /// Loads invoice rows based on the active search filters.
        /// </summary>
        private async Task LoadInvoicesAsync()
        {
            try
            {
                int? selectedInvoiceNumber = cmbInvoiceNumber.SelectedItem is int invoiceNumber ? invoiceNumber : null;
                decimal? selectedTotalCharge = cmbTotalCharge.SelectedItem is decimal totalCharge ? totalCharge : null;
                DateTime? selectedInvoiceDate = dpInvoiceDate.SelectedDate;

                IReadOnlyList<clsInvoiceSummary> invoices = await _searchLogic.LoadInvoicesAsync(
                    selectedInvoiceNumber,
                    selectedInvoiceDate,
                    selectedTotalCharge);

                _invoiceSummaries.Clear();

                foreach (clsInvoiceSummary invoice in invoices)
                {
                    _invoiceSummaries.Add(invoice);
                }

                await UpdateStatusTextAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to load the invoice search results.", ex);
            }
        }

        /// <summary>
        /// Updates the search status text with the current database and result status.
        /// </summary>
        private async Task UpdateStatusTextAsync()
        {
            try
            {
                (bool success, string message) = await _searchLogic.GetDatabaseStatusAsync();
                string resultSummary = $"Showing {_invoiceSummaries.Count} invoice(s).";
                txtSearchStatus.Text = success ? resultSummary : $"{resultSummary} {message}";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to update the search status text.", ex);
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
