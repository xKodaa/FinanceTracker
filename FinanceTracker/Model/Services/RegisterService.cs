using System.Data.SQLite;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;

namespace FinanceTracker.Model.Services
{
    public class RegisterService
    {
        private readonly DatabaseConnector connector;
        private readonly SQLiteConnection connection;
        private readonly DatabaseContentService databaseContentService;

        public RegisterService()
        {
            connector = DatabaseConnector.Instance;
            connection = connector.Connection;
            databaseContentService = new();
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
            using SQLiteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@lastLogin", DateTime.Now);
            int success = command.ExecuteNonQuery();

            if (success == 0)
            {
                Logger.WriteErrorLog(this, $"Registrace uživatele '{username}' selhala");
                Util.ShowErrorMessageBox("Registrace selhala, zkuste to prosím znovu");
                return false;
            }
            Logger.WriteLog(this, $"Uživatel '{username}' byl úspěšně zaregistrován");
            databaseContentService.InitUsersCategories(username);
            return true;
        }
    }
}
