using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceTracker.Config
{
    public class LoginService
    {
        private DatabaseConnector connector;
        private SQLiteConnection connection;
        
        public LoginService() 
        {
            connector = DatabaseConnector.Instance;
            connection = connector.Connection;
        }

        // Zajišťuje uživatelské přihlášení do databáze
        public bool Login(string username, string password) 
        {
            if (!Util.Util.UserExists(username)) 
            {
                Util.Util.ShowErrorMessageBox("Toto uživatelské jméno neexistuje");
                return false;
            }
            string hashedPassword = Util.Util.HashInput(password);
            string sql = "SELECT password FROM Users WHERE username LIKe @username";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                object result = command.ExecuteScalar();

                if (result != null) 
                {
                    string? passwordFromDb = result.ToString();
                    if (passwordFromDb != null && passwordFromDb.Equals(hashedPassword))
                        return true;
                }
            }
            return false;
        }
    }
}
