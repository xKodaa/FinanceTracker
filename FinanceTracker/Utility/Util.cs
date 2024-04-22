using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using Newtonsoft.Json;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace FinanceTracker.Utility
{
    public class Util
    {
        private static readonly SQLiteConnection? connection;
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

        // Editace konfiguračního souboru
        public static void EditAppConfig(string key, object value)
        {
            AppConfig ??= ReadAppConfig();
            if (value == null)
            {
                ShowErrorMessageBox("Hodnota nemůže být null");
                Logger.WriteErrorLog(nameof(Util), $"Chyba při editaci app_config.json souboru: key={key}, value=null");
                return;
            }
            try
            {
                switch (key)
                {
                    case "ConnectionString":
                        AppConfig.ConnectionString = (string)value;
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
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));

            StringBuilder builder = new();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        // Vyčtení, zda se uživatel již nenachází v databázi -> pro zabránění duplicitních uživatelských jmen
        public static bool UserExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE username LIKE @username";
            using SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            object result = command.ExecuteScalar();

            if (result != null)
            {
                if (int.TryParse(result.ToString(), out int count))
                {
                    return count > 0;
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

        // Kontrola, zda je datum v minulosti
        public static bool NonFutureDateTime(DateTime dateTime)
        {
            return dateTime <= DateTime.Now;
        }

        // Nastavení uživatele singleton connectoru
        public static void SetUser(User user)
        {
            Connector = DatabaseConnector.Instance;
            Connector.LoggedUser = user;
            Logger.WriteLog(nameof(Util), $"Uživatel byl úspěšně nastaven '{user}'");
        }

        public static User GetUser()
        {
            return Connector.LoggedUser;
        }

        // Načtení uživatele z databáze
        public static User LoadUser()
        {
            string username = GetUser().Username;
            string sql = "SELECT name, surname, lastLogin FROM Users WHERE username LIKE @username";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                using SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new User(username)
                    {
                        Name = reader["name"].ToString(),
                        Surname = reader["surname"].ToString(),
                        LastLogin = DateTime.Parse(reader["lastLogin"].ToString())
                    };
                }
            }
            return new User(username);
        }

        // Získání kvartálů pro aktuální rok
        public static List<Quart> GetQuarts()
        {
            int year = DateTime.Now.Year;

            Quart quart1 = new(1, new DateTime(year, 1, 1), new DateTime(year, 3, 31));
            Quart quart2 = new(2, new DateTime(year, 4, 1), new DateTime(year, 6, 30));
            Quart quart3 = new(3, new DateTime(year, 7, 1), new DateTime(year, 9, 30));
            Quart quart4 = new(4, new DateTime(year, 10, 1), new DateTime(year, 12, 31));

            return [quart1, quart2, quart3, quart4];
        }
    }
}
