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
        private SQLiteConnection? Connection { get; set; }

        private static DatabaseConnector? databaseConnector;

        public static DatabaseConnector Instance    // Singletone pro získávání stejné instance připojení do DB
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

        public DatabaseConnector()
        {
            ConnectionString = String.Empty;
            ConnectionPath = "Connection.json";
            Connection = null;
            CreateDatabaseConnection();
        }

        private void CreateDatabaseConnection()
        {
            ConnectionString = LoadDatabaseConnectionString();
            if (ConnectionString != null) 
            {
                Connection = new SQLiteConnection(ConnectionString);
            }
            else 
            {
                MessageBox.Show("Connection not created, shutting down...", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Configuration file not found in project folder.", "Error", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not read a configurtaion file {ConnectionPath}: " + ex.Message, "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
    }
}
