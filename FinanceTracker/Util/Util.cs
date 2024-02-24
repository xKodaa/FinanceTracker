﻿using FinanceTracker.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceTracker.Util
{
    public class Util
    {
        private static SQLiteConnection? connection;

        static Util() 
        {
            DatabaseConnector connector = DatabaseConnector.Instance;
            connection = connector.Connection;
        }

        // SHA256 hashování řetěžce
        public static String HashInput(string input) 
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

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

        public static void ShowErrorMessageBox(string input) 
        {
            MessageBox.Show(input, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfoMessageBox(string input) 
        {
            MessageBox.Show(input, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool ValidLoginOrRegistrationInputs(string username, string password)
        {
            if (string.IsNullOrEmpty(password) && string.IsNullOrEmpty(username))
            {
                ShowErrorMessageBox("Please fill both values!");
                return false;
            }
            if (string.IsNullOrEmpty(username))
            {
                ShowErrorMessageBox("Please fill username!");
                return false;
            }
            if (string.IsNullOrEmpty(password))
            {
                ShowErrorMessageBox("Please fill password!");
                return false;
            }
            return true;
        }
    }
}
