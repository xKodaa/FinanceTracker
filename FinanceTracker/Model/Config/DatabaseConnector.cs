using FinanceTracker.Utility;
using System.Data.SQLite;

namespace FinanceTracker.Model.Config
{
    public class DatabaseConnector
    {
        private string? ConnectionString { get; set; }
        public SQLiteConnection Connection { get; set; }

        private static DatabaseConnector? databaseConnector;

        public User LoggedUser { get; set; }

        // Singletone pro získávání stejné instance připojení do DB
        public static DatabaseConnector Instance
        {
            get
            {
                if (databaseConnector == null)
                {
                    databaseConnector = new DatabaseConnector();
                }
                return databaseConnector;
            }
        }

        private DatabaseConnector()
        {
            ConnectionString = string.Empty;
            Connection = new SQLiteConnection();
            LoggedUser = new User("default")
            {
                Name = "default",
                Surname = "default",
                LastLogin = DateTime.Now
            };

            CreateDatabaseConnection();
        }

        // Vytvoření spojení k SQLite databázi
        private void CreateDatabaseConnection()
        {
            ConnectionString = LoadDatabaseConnectionString();
            if (ConnectionString != null)
            {
                Connection.ConnectionString = ConnectionString;
                try
                {
                    Connection.Open();
                }
                catch (Exception ex)
                {
                    Logger.WriteErrorLog(this, $"Připojení k databázi se nepodařilo vytvořit, {ex.Message} \n - Konec aplikace");
                    Util.ShowErrorMessageBox("Připojení k databázi se nepodařilo vytvořit");
                    Environment.Exit(0);
                }
            }
            else
            {
                Logger.WriteErrorLog(this, "Připojení k databázi se nepodařilo vytvořit, protože ConnectionString je null, konec aplikace");
                Util.ShowErrorMessageBox("Připojení k databázi se nepodařilo vytvořit, ukončuji aplikaci...");
                Environment.Exit(0);
            }
        }

        // Načtení connection stringu z AppConfig.json
        private string? LoadDatabaseConnectionString()
        {
            AppConfig config = Util.ReadAppConfig();
            return config.ConnectionString;
        }
    }
}
