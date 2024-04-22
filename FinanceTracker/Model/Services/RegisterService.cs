using System.Data.SQLite;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;

namespace FinanceTracker.Model.Services
{
    public class RegisterService
    {
        private readonly DatabaseConnector connector;
        private readonly SQLiteConnection connection;
        private readonly string[] categories = { "Potraviny", "Bydlení", "Zdravotní péče", "Doprava", "Vzdělání", "Zábava a volný čas",
        "Oblečení a osobní péče", "Děti a péče o rodinu", "Restaurace a stravování venku", "Spoření a investice" };

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
            using SQLiteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@lastLogin", DateTime.Now);
            command.ExecuteNonQuery();
            InitUsersCategories(username);
            return true;
        }

        private void InitUsersCategories(string username)
        {
            string sql = "INSERT INTO UserCategories (username, category) VALUES (@username, @category)";

            using SQLiteCommand command = new(sql, connection);

            foreach (var category in categories)
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@category", category);
                command.ExecuteNonQuery();  
                command.Parameters.Clear(); 
            }
        }
    }
}
