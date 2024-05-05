using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System.Data.SQLite;

namespace FinanceTracker.Model.Services
{
    public class DatabaseContentService
    {
        private readonly DatabaseConnector connector;
        private readonly SQLiteConnection connection;
        private readonly List<string> TablesToCheck = ["Users", "UserCryptos", "UserFinances", "UserCategories"];
        private readonly string[] Categories = [ "Potraviny", "Bydlení", "Zdravotní péče", "Doprava", "Vzdělání", "Zábava a volný čas",
        "Oblečení a osobní péče", "Děti a péče o rodinu", "Restaurace a stravování venku", "Spoření a investice" ];

        public DatabaseContentService()
        {
            connector = DatabaseConnector.Instance;
            connection = connector.Connection;
            CheckDatabaseContent();
        }

        // Kontrola obsahu uživatelské sqlite databáze
        public void CheckDatabaseContent()
        {
            Logger.WriteLog(this, "Kontrola obsahu databáze...");
            foreach (var table in TablesToCheck)
            {
                // SQL dotaz pro zjištění, zda tabulka existuje
                string sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName;";
                using var command = new SQLiteCommand(sql, connection);
                command.Parameters.AddWithValue("@tableName", table);
                var result = command.ExecuteScalar();
                if (result == null)
                {
                    Logger.WriteErrorLog(this, $"Tabulka '{table}' neexistuje.");
                    CreateNonExistingTable(table);
                }
                else
                {
                    Logger.WriteLog(this, $"Tabulka '{table}' existuje. Není potřeba vytvářet");
                }
            }
        }

        // Vytvoření tabulky, pokud neexistuje
        private void CreateNonExistingTable(string table)
        {
            string sql = table switch
            {
                "Users" => "CREATE TABLE Users (username TEXT PRIMARY KEY, password TEXT, name TEXT, surname TEXT, lastLogin DATETIME)",
                "UserCryptos" => "CREATE TABLE UserCryptos (username TEXT, cryptoName TEXT, amount INTEGER, dateOfBuy DATETIME, price INT, FOREIGN KEY(username) REFERENCES Users(username))",
                "UserFinances" => "CREATE TABLE UserFinances (username TEXT, category TEXT, date DATETIME, price NUMERIC, FOREIGN KEY(username) REFERENCES Users(username))",
                "UserCategories" => "CREATE TABLE UserCategories (username TEXT, category TEXT, FOREIGN KEY(username) REFERENCES Users(username))",
                _ => "",
            };
            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
            Util.ShowInfoMessageBox($"Tabulka {table} byla vytvořena.");
        }

        public void InitUsersCategories(string username)
        {
            string sql = "INSERT INTO UserCategories (username, category) VALUES (@username, @category)";

            using SQLiteCommand command = new(sql, connection);

            foreach (var category in Categories)
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@category", category);
                int success = command.ExecuteNonQuery();
                if (success == 0)
                {
                    Logger.WriteErrorLog(this, $"Nepodařilo se vytvořit kategorii '{category}' pro uživatele '{username}'");
                    Util.ShowErrorMessageBox($"Nepodařilo se vytvořit kategorii '{category}'");
                }
                command.Parameters.Clear();
            }
        }
    }
}
