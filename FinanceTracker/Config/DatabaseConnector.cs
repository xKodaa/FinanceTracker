using FinanceTracker.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceTracker.Config
{
    public class DatabaseConnector
    {
        private string? ConnectionString { get; set; }
        private string ConnectionPath { get; set; }
        public SQLiteConnection Connection { get; set; }

        private static DatabaseConnector? databaseConnector;

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
            ConnectionString = String.Empty;
            ConnectionPath = "Connection.json";
            Connection = new SQLiteConnection();
            CreateDatabaseConnection();
        }

        // Vytvoření spojení k SQLite databázi
        private void CreateDatabaseConnection()
        {
            ConnectionString = LoadDatabaseConnectionString();
            if (ConnectionString != null) 
            {
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
            else 
            {
                Util.Util.ShowErrorMessageBox("Připojení k databázi se nepodařilo vytvořit, ukončení aplikace...");
                Environment.Exit(0);
            }
        }

        // 'bin/debug/net8.0-windows' je kořenová složka pro čtení ze souborů
        // proto se zde nachází jak databázový soubor, tak konfigurační hodnoty
        private string? LoadDatabaseConnectionString()  
        {
            try
            {
                if (File.Exists(ConnectionPath))
                {
                    string json = File.ReadAllText(ConnectionPath);
                    DatabaseConfiguration? config = JsonConvert.DeserializeObject<DatabaseConfiguration>(json);
                    if (config != null)
                    {
                        string connString = config.ConnectionString;
                        if (connString != null)
                        {
                            return connString;
                        }
                    }
                    return null;
                }
                else
                {
                    Util.Util.ShowErrorMessageBox("Konfigurační soubor nenalezen v souborech aplikace");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Util.Util.ShowErrorMessageBox($"Nepodařilo se přečíst konfigurační soubor {ConnectionPath}: " + ex.Message);
                return null;
            }
        }
    }
}
