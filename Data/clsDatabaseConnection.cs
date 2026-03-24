using System.Data.OleDb;
using System.IO;

namespace Group_Assignment.Data
{
    /// <summary>
    /// Creates and validates Access database connections.
    /// </summary>
    public sealed class clsDatabaseConnection
    {
        private readonly clsDatabaseSettings _databaseSettings;
        private string? _resolvedProviderName;

        /// <summary>
        /// Initializes a new instance of the <see cref="clsDatabaseConnection"/> class.
        /// </summary>
        /// <param name="databaseSettings">The database settings to use.</param>
        public clsDatabaseConnection(clsDatabaseSettings? databaseSettings = null)
        {
            try
            {
                _databaseSettings = databaseSettings ?? new clsDatabaseSettings();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to initialize the database connection helper.", ex);
            }
        }

        /// <summary>
        /// Gets the configured database file path.
        /// </summary>
        public string DatabaseFilePath
        {
            get
            {
                try
                {
                    return _databaseSettings.DatabaseFilePath;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to get the database file path.", ex);
                }
            }
        }

        /// <summary>
        /// Determines whether the configured Access database file exists.
        /// </summary>
        /// <returns><c>true</c> when the file exists; otherwise <c>false</c>.</returns>
        public bool DatabaseFileExists()
        {
            try
            {
                return File.Exists(DatabaseFilePath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to verify the database file.", ex);
            }
        }

        /// <summary>
        /// Builds an OleDb connection string for the requested provider.
        /// </summary>
        /// <param name="providerName">The Access OLE DB provider name.</param>
        /// <returns>A valid connection string.</returns>
        public string BuildConnectionString(string providerName)
        {
            try
            {
                OleDbConnectionStringBuilder connectionStringBuilder = new()
                {
                    Provider = providerName,
                    DataSource = DatabaseFilePath,
                };

                connectionStringBuilder.Add("Persist Security Info", false);

                return connectionStringBuilder.ConnectionString;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to build the Access connection string.", ex);
            }
        }

        /// <summary>
        /// Resolves a usable Access provider for the configured database.
        /// </summary>
        /// <returns>The provider name to use for future connections.</returns>
        public async Task<string> ResolveProviderNameAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_resolvedProviderName))
                {
                    return _resolvedProviderName;
                }

                if (!DatabaseFileExists())
                {
                    _resolvedProviderName = _databaseSettings.PreferredProviders[0];
                    return _resolvedProviderName;
                }

                foreach (string providerName in _databaseSettings.PreferredProviders)
                {
                    try
                    {
                        using OleDbConnection connection = new(BuildConnectionString(providerName));
                        await connection.OpenAsync().ConfigureAwait(false);
                        _resolvedProviderName = providerName;
                        return _resolvedProviderName;
                    }
                    catch
                    {
                    }
                }

                _resolvedProviderName = _databaseSettings.PreferredProviders[0];
                return _resolvedProviderName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to resolve an Access OLE DB provider.", ex);
            }
        }

        /// <summary>
        /// Creates a new OleDb connection for the configured database.
        /// </summary>
        /// <param name="providerName">The provider name to use, or null to use the resolved provider.</param>
        /// <returns>A new OleDb connection.</returns>
        public OleDbConnection CreateConnection(string? providerName = null)
        {
            try
            {
                string connectionProvider = providerName ?? _resolvedProviderName ?? _databaseSettings.PreferredProviders[0];
                return new OleDbConnection(BuildConnectionString(connectionProvider));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to create the Access database connection.", ex);
            }
        }

        /// <summary>
        /// Tests the configured database connection and returns a user-friendly result.
        /// </summary>
        /// <returns>A tuple describing the connection status.</returns>
        public async Task<(bool Success, string Message)> TestConnectionAsync()
        {
            try
            {
                if (!DatabaseFileExists())
                {
                    return (false, $"Database file not found: {DatabaseFilePath}");
                }

                List<string> errorMessages = new();

                foreach (string providerName in _databaseSettings.PreferredProviders)
                {
                    try
                    {
                        using OleDbConnection connection = new(BuildConnectionString(providerName));
                        await connection.OpenAsync().ConfigureAwait(false);
                        _resolvedProviderName = providerName;

                        return (true, $"Connected to {Path.GetFileName(DatabaseFilePath)} using {providerName}.");
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add($"{providerName}: {ex.Message}");
                    }
                }

                return
                (
                    false,
                    "Database file found, but no compatible ACE provider was available. " +
                    $"Tried: {string.Join(" | ", errorMessages)}"
                );
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to test the Access database connection.", ex);
            }
        }
    }
}
