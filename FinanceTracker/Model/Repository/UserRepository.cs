using FinanceTracker.Model.Config;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Repository
{
    class UserRepository
    {
        public bool Register(string name, string surname, string username, string password, Currency currency)
        {
            if (UserExists(username))
            {
                Logger.WriteErrorLog(this, $"Uživatel se pokusil zaregistrovat s existujícím jménem '{username}'");
                Util.ShowErrorMessageBox("Uživatel s tímto uživatelským jménem již existuje, prosím, použijte jiné");
                return false;
            }
            string hashedPassword = Util.HashInput(password);
            string sql = "INSERT INTO Users Values(@username, @password, @name, @surname, @lastLogin, @currency)";
            using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@surname", surname);
            command.Parameters.AddWithValue("@lastLogin", DateTime.Now);
            command.Parameters.AddWithValue("@currency", currency.Id);
            int success = command.ExecuteNonQuery();

            if (success == 0)
            {
                Logger.WriteErrorLog(this, $"Registrace uživatele '{username}' selhala");
                Util.ShowErrorMessageBox("Registrace selhala, zkuste to prosím znovu");
                return false;
            }
            Logger.WriteLog(this, $"Uživatel '{username}' byl úspěšně zaregistrován");
            new UserExpenseCategoryRepository().CreateCategoryForUser(username);
            return true;
        }

        public bool UserExists(string username)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE username LIKE @username";
            using SQLiteCommand command = new SQLiteCommand(sql, DatabaseConnector.Instance.Connection);
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

        // Načtení uživatele z databáze
        public User LoadUser()
        {
            string username = UserInfoService.GetUser().Username;
            string sql = @"SELECT u.name, u.surname, u.lastLogin, c.id AS currencyId, c.code AS currencyCode, c.name AS currencyName
               FROM users u LEFT JOIN currencies c ON u.currency_id = c.id
               WHERE u.username LIKE @username";

            using (SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection))
            {
                command.Parameters.AddWithValue("@username", username);
                using SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Currency? currency = null;

                    if (!reader.IsDBNull(reader.GetOrdinal("currencyId")))
                    {
                        currency = new Currency
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("currencyId")),
                            Code = reader.GetString(reader.GetOrdinal("currencyCode")),
                            Name = reader.GetString(reader.GetOrdinal("currencyName"))
                        };
                    }

                    return new User(username)
                    {
                        Name = reader["name"].ToString(),
                        Surname = reader["surname"].ToString(),
                        LastLogin = DateTime.Parse(reader["lastLogin"].ToString()),
                        Currency = currency
                    };
                }
            }
            return new User(username);
        }
    }
}
