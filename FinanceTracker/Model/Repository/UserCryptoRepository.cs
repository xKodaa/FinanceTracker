using FinanceTracker.Model.Config;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Repository
{
    class UserCryptoRepository
    {
        private static string TableName = "user_cryptos";

        public int GetUsersNumberOfCryptos(string username)
        {
            string sql = $"SELECT COUNT(DISTINCT cryptoName) FROM {TableName} WHERE username=@username";
            using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);
            command.Parameters.AddWithValue("@username", username);
            object result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }
    }
}
