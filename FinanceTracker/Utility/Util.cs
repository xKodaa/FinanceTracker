using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceTracker.Utility
{
    public class Util
    {
        private static SQLiteConnection? connection;
        private static AppConfig AppConfig { get; set; }
        private static readonly string AppConfigPath;
        private static DatabaseConnector Connector;

        static Util() 
        {
            AppConfigPath = "Data/app_config.json";
            AppConfig = ReadAppConfig();
            Connector = DatabaseConnector.Instance;
            connection = Connector.Connection;
        }

        // Přečtení konfiguračního souboru
        // 'bin/debug/net8.0-windows' je kořenová složka pro čtení ze souborů
        // proto se zde nachází jak databázový soubor, tak konfigurační hodnoty
        public static AppConfig ReadAppConfig()
        {
            if (AppConfig == null) 
            {
                if (!File.Exists(AppConfigPath))
                {
                    ShowErrorMessageBox("Konfigurační soubor nenalezen");
                    Logger.WriteErrorLog(nameof(Util), $"Konfigurační soubor {AppConfigPath} nenalezen ve složce /bin/debug/net8.0-windows/");
                    Environment.Exit(0);
                }

                AppConfig = new AppConfig();
                try 
                {
                    string content = File.ReadAllText(AppConfigPath);
                    AppConfig? config = JsonConvert.DeserializeObject<AppConfig>(content);
                    if (config != null && config.IsInitialized())
                    {
                        AppConfig.ConnectionString = config.ConnectionString;
                        AppConfig.DefaultCurrency = config.DefaultCurrency;
                        AppConfig.CryptoRefreshRate = config.CryptoRefreshRate;
                        AppConfig.FinanceCategories = config.FinanceCategories;
                    }
                    Logger.WriteLog(nameof(Util), AppConfig.ToString());
                    return AppConfig;
                } catch (Exception ex)
                {
                    ShowErrorMessageBox($"Nepodařilo se načíst konfigurační soubor {AppConfigPath}!");
                    Logger.WriteErrorLog(nameof(Util), AppConfig.ToString());
                    Logger.WriteErrorLog(nameof(Util), $"Konfigurační soubor {AppConfigPath} se nepodařilo načíst, chyba: {ex.Message} \n - Konec aplikace");
                    Environment.Exit(0);
                }
            }
            return AppConfig;
        }

        public static void EditAppConfig(string key, object value)
        {
            if (AppConfig == null)
            {
                AppConfig = ReadAppConfig();
            }
            try
            {
                switch (key)
                {
                    case "ConnectionString":
                        AppConfig.ConnectionString = value.ToString();
                        break;
                    case "DefaultCurrency":
                        AppConfig.DefaultCurrency = value.ToString();
                        break;
                    case "CryptoRefreshRate":
                        if (int.TryParse(value.ToString(), out int rate))
                        {
                            AppConfig.CryptoRefreshRate = rate;
                        }
                        else
                        {
                            ShowErrorMessageBox("Zadejte prosím číslo");
                            Logger.WriteErrorLog(nameof(Util), $"Chyba při editaci app_config.json souboru: key={key}, value={value}");
                            return;
                        }
                        break;
                    case "FinanceCategories":
                        List<string> categories = (List<string>)value;
                        AppConfig.FinanceCategories = categories;
                        break;

                }   
                string json = JsonConvert.SerializeObject(AppConfig, Formatting.Indented);
                File.WriteAllText(AppConfigPath, json);
                Logger.WriteLog(nameof(Util), "Konfigurační soubor byl úspěšně upraven");
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Nepodařilo se upravit konfigurační soubor");
                Logger.WriteErrorLog(nameof(Util), $"Nepodařilo se upravit konfigurační soubor {AppConfigPath}, chyba: {ex.Message}");
            }
        }   

        // SHA256 hashování řetěžce
        public static String HashInput(string input) 
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Vyčtení, zda se uživatel již nenachází v databázi -> pro zabránění duplicitních uživatelských jmen
        public static bool UserExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE username LIKE @username";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    if (int.TryParse(result.ToString(), out int count))
                    {
                        return count > 0;
                    }
                }
            }
            return false;
        }

        /* UTILITY pro zjednodušené volání různých typů message boxů */
        public static void ShowErrorMessageBox(string input) 
        {
            MessageBox.Show(input, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfoMessageBox(string input) 
        {
            MessageBox.Show(input, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Kontrola uživatelských vstupů
        public static bool ValidLoginOrRegistrationInputs(string name, string surname, string username, string password)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(surname) && string.IsNullOrEmpty(username) && string.IsNullOrEmpty(username))
            {
                ShowErrorMessageBox("Vyplňte prosím všechny hodnoty!");
                return false;
            }
            if (string.IsNullOrEmpty(name))
            {
                ShowErrorMessageBox("Vyplňte prosím křestní jméno!");
                return false;
            }
            if (string.IsNullOrEmpty(surname))
            {
                ShowErrorMessageBox("Vyplňte prosím příjmení!");
                return false;
            }            
            if (string.IsNullOrEmpty(username))
            {
                ShowErrorMessageBox("Vyplňte prosím uživatelské jméno!");
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                ShowErrorMessageBox("Vyplňte prosím heslo!");
                return false;
            }
            return true;
        }

        private void GetTickersFromCsv()
        {
            string sourceFilePath = "Data/tickers_full.csv";
            string destinationFilePath = "Data/tickers.csv";
            try
            {
                using (StreamReader reader = new StreamReader(sourceFilePath))
                using (StreamWriter writer = new StreamWriter(destinationFilePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string? line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] values = line.Split(',');
                            string row = values[0] + ";" + values[1];
                            writer.WriteLine(row);
                        }
                    }
                }
                Logger.WriteLog(nameof(Util), "Tickers byly úspěšně přepsány");
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(nameof(Util), $"Chyba při príci se soubroy Tickers: {ex.Message}");
                ShowErrorMessageBox("Chyba při práci se soubory Tickers");
            }
        }

        public static void SetUser(string username)
        {
            Connector = DatabaseConnector.Instance;
            Connector.LoggedUser = new User(username);
            Logger.WriteLog(nameof(Util), $"Uživatel '{username}' byl úspěšně nastaven");
        }

        public static User GetUser()
        {
            return Connector.LoggedUser;
        }
    }
}
