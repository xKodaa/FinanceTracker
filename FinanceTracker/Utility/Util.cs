﻿using FinanceTracker.Config;
using FinanceTracker.Model;
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
        private static string AppConfigPath;

        static Util() 
        {
            AppConfigPath = "AppConfig.json";
            AppConfig = ReadAppConfig();
            DatabaseConnector connector = DatabaseConnector.Instance;
            connection = connector.Connection;
        }

        // Přečtení konfiguračního souboru
        // 'bin/debug/net8.0-windows' je kořenová složka pro čtení ze souborů
        // proto se zde nachází jak databázový soubor, tak konfigurační hodnoty
        // tm-fb2e32a1-6465-4218-8e40-be1e9507de67
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
                        AppConfig.StocksRefreshRate = config.StocksRefreshRate;
                        AppConfig.CryptoRefreshRate = config.CryptoRefreshRate;
                    }
                    Logger.WriteErrorLog(nameof(Util), $"ConnectionString = {config?.ConnectionString}, " +
                        $"StocksRefreshRate = {config?.StocksRefreshRate}, CryptoRefreshRate = {config?.CryptoRefreshRate}");
                    return AppConfig;
                } catch (Exception ex)
                {
                    ShowErrorMessageBox($"Nepodařilo se načíst konfigurační soubor {AppConfigPath}!");
                    Logger.WriteErrorLog(nameof(Util), $"Konfigurační soubor {AppConfigPath} se nepodařilo načíst, chyba: {ex.Message} \n - Konec aplikace");
                    Environment.Exit(0);
                }
            }
            Logger.WriteErrorLog(nameof(Util), $"Konfigurační soubor {AppConfigPath} nebyl správně načten");
            return AppConfig;
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
    }
}