﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FinanceTracker.Util;

namespace FinanceTracker.Config
{
    public class RegisterService
    {

        private DatabaseConnector connector;
        private SQLiteConnection connection;

        public RegisterService()
        {
            connector = DatabaseConnector.Instance;
            connection = connector.Connection;
        }

        // Zajišťuje uživatelskou registraci do databáze
        public bool Register(string username, string password)
        {
            if (Util.Util.UserExists(username)) 
            {
                Util.Util.ShowErrorMessageBox("Uživatel s tímto uživatelským jménem již existuje, prosím, použijte jiné");
                return false;
            }
            string hashedPassword = Util.Util.HashInput(password);
            string sql = "INSERT INTO Users Values(null, @username, @password)";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.ExecuteNonQuery();
                return true;
            }
        }
    }
}