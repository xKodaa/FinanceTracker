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

        public bool Register(string username, string password)
        {
            if (Util.Util.UserExists(username)) 
            {
                MessageBox.Show("This username already exists, please choose another one", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
