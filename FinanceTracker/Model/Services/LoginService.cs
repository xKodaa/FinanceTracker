using FinanceTracker.Model.Config;
using FinanceTracker.Model.Repository;
using FinanceTracker.Utility;
using System.Data.SQLite;

namespace FinanceTracker.Model.Services
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
        public bool Login(string username, string password, out bool userExists)
        {
            if (!new UserRepository().UserExists(username))
            {
                Util.ShowErrorMessageBox("Toto uživatelské jméno neexistuje");
                Logger.WriteErrorLog(this, $"Uživatel použil při přihlášení neznámé uživatelské jméno '{username}'");
                userExists = false;
                return false;
            }
            string hashedPassword = Util.HashInput(password);
            string sql = "SELECT password FROM Users WHERE username LIKE @username";
            using (SQLiteCommand command = new(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    string? passwordFromDb = result.ToString();
                    if (passwordFromDb != null && passwordFromDb.Equals(hashedPassword))
                    {
                        userExists = true;
                        return true;
                    }

                }
            }
            userExists = true;
            return false;
        }
    }
}
