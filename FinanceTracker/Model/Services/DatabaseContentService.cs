using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System.Data.SQLite;

namespace FinanceTracker.Model.Services
{
    public class DatabaseContentService
    {
        private readonly DatabaseConnector connector;
        private readonly SQLiteConnection connection;
        private readonly List<string> TablesToCheck = ["users", "user_cryptos", "user_expenses", "user_expense_categories"];


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
                "users" => "CREATE TABLE users (username TEXT PRIMARY KEY, password TEXT, name TEXT, surname TEXT, lastLogin DATETIME)",
                "user_cryptos" => "CREATE TABLE user_cryptos (username TEXT, cryptoName TEXT, amount INTEGER, dateOfBuy DATETIME, price INT, FOREIGN KEY(username) REFERENCES Users(username))",
                "user_expenses" => "CREATE TABLE user_expenses (username TEXT, category TEXT, date DATETIME, price NUMERIC, FOREIGN KEY(username) REFERENCES Users(username))",
                "user_expense_categories" => "CREATE TABLE user_expense_categories (username TEXT, category TEXT, FOREIGN KEY(username) REFERENCES Users(username))",
                "currencies" => "CREATE TABLE currencies (id INTEGER PRIMARY KEY AUTOINCREMENT, code TEXT UNIQUE, name TEXT)",
                _ => "",
            };
            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
            Util.ShowInfoMessageBox($"Tabulka {table} byla vytvořena.");
        }
    }
}
