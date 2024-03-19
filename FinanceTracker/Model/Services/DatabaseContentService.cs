using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Services
{
    public class DatabaseContentService
    {
        private readonly DatabaseConnector connector;
        private readonly SQLiteConnection connection;
        private readonly List<string> TablesToCheck;

        public DatabaseContentService()
        {
            connector = DatabaseConnector.Instance;
            connection = connector.Connection;
            TablesToCheck = ["Users", "UserCryptos", "UserFinances"];
            CheckDatabaseContent();
        }

        private void CheckDatabaseContent()
        {
            Logger.WriteLog(this, "Kontrola obsahu databáze...");
            foreach (var table in TablesToCheck)
            {
                // SQL dotaz pro zjištění, zda tabulka existuje
                string sql = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{table}';";
                using (var command = new SQLiteCommand(sql, connection))
                {
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
        }

        private void CreateNonExistingTable(string table)
        {
            string sql;
            switch (table)
            {
                case "Users":
                    sql = "CREATE TABLE Users (username TEXT PRIMARY KEY, password TEXT, name TEXT, surname TEXT)";
                    break;
                case "UserStocks":
                    sql = "CREATE TABLE UserFinances (username TEXT, category TEXT, date DATETIME, price REAL, FOREIGN KEY(username) REFERENCES Users(username))";
                    break;
                case "UserCryptos":
                    sql = "CREATE TABLE UserCryptos (username TEXT, cryptoName TEXT, amount INTEGER, dateOfBuy DATETIME, FOREIGN KEY(username) REFERENCES Users(username))";
                    break;
                default:
                    sql = "";
                    break;
            }

            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
                Util.ShowInfoMessageBox($"Tabulka {table} byla vytvořena.");
            }
        }
    }
}
