using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Repository
{
    class UserCryptoRepository
    {
        private static string TableName = "user_cryptos";
        private static SQLiteConnection Connection => DatabaseConnector.Instance.Connection;

        public int GetUsersNumberOfCryptos(string username)
        {
            string sql = $"SELECT COUNT(DISTINCT cryptoName) FROM {TableName} WHERE username=@username";
            using SQLiteCommand command = new(sql, Connection);
            command.Parameters.AddWithValue("@username", username);
            object result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public List<UserCryptoCurrency> GetUserCryptos(string username)
        {
            List<UserCryptoCurrency> results = [];

            try
            {
                string sql = $"SELECT cryptoName, amount, dateOfBuy, price FROM {TableName} WHERE username = @username";
                using SQLiteCommand command = new(sql, Connection)
                {
                    Parameters = { new SQLiteParameter("@username", username) }
                };

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string cryptoName = reader.GetString(0);
                    decimal amount = reader.GetDecimal(1);
                    DateTime dateOfBuy = reader.GetDateTime(2);
                    decimal price = reader.GetDecimal(3);

                    results.Add(new UserCryptoCurrency(cryptoName, amount, price, dateOfBuy));
                }
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(nameof(UserCryptoRepository), $"Nepodařilo se načíst kryptoměny: {ex.Message}");
                Util.ShowErrorMessageBox("Nepodařilo se načíst kryptoměny z databáze.");
            }

            return results;
        }

        public bool AddUserCrypto(UserCryptoCurrency crypto, string username)
        {
            try
            {
                string sql = $"INSERT INTO {TableName} (username, cryptoName, amount, dateOfBuy, price) VALUES (@username, @cryptoName, @amount, @dateOfBuy, @price)";
                using SQLiteCommand command = new(sql, Connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@cryptoName", crypto.Symbol);
                command.Parameters.AddWithValue("@amount", crypto.Amount);
                command.Parameters.AddWithValue("@dateOfBuy", crypto.DateOfBuy);
                command.Parameters.AddWithValue("@price", crypto.PricePerAmount);
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(nameof(UserCryptoRepository), $"Chyba při ukládání kryptoměny: {ex.Message}");
                return false;
            }
        }

        public void DeleteUserCrypto(UserCryptoCurrency crypto, string username)
        {
            try
            {
                string sql = $"DELETE FROM {TableName} WHERE username=@username AND cryptoName=@cryptoName AND amount=@amount AND dateOfBuy=@dateOfBuy AND price=@price";
                using SQLiteCommand command = new(sql, Connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@cryptoName", crypto.Symbol);
                command.Parameters.AddWithValue("@amount", crypto.Amount);
                command.Parameters.AddWithValue("@dateOfBuy", crypto.DateOfBuy);
                command.Parameters.AddWithValue("@price", crypto.PricePerAmount);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(nameof(UserCryptoRepository), $"Chyba při mazání kryptoměny: {ex.Message}");
            }
        }
    }
}
