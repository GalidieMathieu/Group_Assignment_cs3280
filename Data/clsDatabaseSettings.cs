using System.Collections.ObjectModel;
using System.IO;

namespace Group_Assignment.Data
{
    /// <summary>
    /// Stores the shared Access database settings for the application.
    /// </summary>
    public sealed class clsDatabaseSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="clsDatabaseSettings"/> class.
        /// </summary>
        public clsDatabaseSettings()
        {
            try
            {
                ProjectRootPath = ResolveProjectRoot();
                DatabaseFolderPath = Path.Combine(ProjectRootPath, "Database");
                DatabaseFilePath = Path.Combine(DatabaseFolderPath, "WeberAccounting.accdb");
                PreferredProviders = new ReadOnlyCollection<string>(
                    new[]
                    {
                        "Microsoft.ACE.OLEDB.16.0",
                        "Microsoft.ACE.OLEDB.12.0",
                    });
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to initialize the database settings.", ex);
            }
        }

        /// <summary>
        /// Gets the project root path.
        /// </summary>
        public string ProjectRootPath { get; }

        /// <summary>
        /// Gets the database folder path.
        /// </summary>
        public string DatabaseFolderPath { get; }

        /// <summary>
        /// Gets the Access database file path.
        /// </summary>
        public string DatabaseFilePath { get; }

        /// <summary>
        /// Gets the Access OLE DB providers to try in order.
        /// </summary>
        public IReadOnlyList<string> PreferredProviders { get; }

        /// <summary>
        /// Resolves the project root from the current application base directory.
        /// </summary>
        /// <returns>The resolved project root path.</returns>
        private static string ResolveProjectRoot()
        {
            try
            {
                DirectoryInfo? currentDirectory = new(AppContext.BaseDirectory);

                while (currentDirectory is not null)
                {
                    string projectFilePath = Path.Combine(currentDirectory.FullName, "Group_Assignment.csproj");

                    if (File.Exists(projectFilePath))
                    {
                        return currentDirectory.FullName;
                    }

                    currentDirectory = currentDirectory.Parent;
                }

                return AppContext.BaseDirectory;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to resolve the project root path.", ex);
            }
        }
    }
}
