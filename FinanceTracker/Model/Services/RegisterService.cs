using System.Data.SQLite;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;

namespace FinanceTracker.Model.Services
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
        public bool Register(string name, string surname, string username, string password)
        {
            if (Util.UserExists(username))
            {
                Logger.WriteErrorLog(this, $"Uživatel se pokusil zaregistrovat s existujícím jménem '{username}'");
                Util.ShowErrorMessageBox("Uživatel s tímto uživatelským jménem již existuje, prosím, použijte jiné");
                return false;
            }
            string hashedPassword = Util.HashInput(password);
            string sql = "INSERT INTO Users Values(@username, @password, @name, @surname, @lastLogin)";
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashedPassword);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@surname", surname);
                command.Parameters.AddWithValue("@lastLogin", DateTime.Now);
                command.ExecuteNonQuery();
                return true;
            }
        }
    }
}
