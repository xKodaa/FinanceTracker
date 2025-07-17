using FinanceTracker.Model.Config;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Repository
{
    class CurrencyRepository
    {
        private static string TableName => "currencies";
        private static SQLiteConnection Connection => DatabaseConnector.Instance.Connection;

        public List<Currency> GetAllCurrencies()
        {
            List<Currency> currencies = new();

            string query = $"SELECT id, code, name FROM {TableName} ORDER BY name ASC";

            using SQLiteCommand command = new(query, Connection);
            using SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                currencies.Add(new Currency
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Code = reader["code"].ToString(),
                    Name = reader["name"].ToString()
                });
            }

            return currencies;
        }

        public Currency? GetCurrencyById(int id)
        {
            string query = $"SELECT id, code, name FROM {TableName} WHERE id = @id";

            using SQLiteCommand command = new(query, Connection);
            command.Parameters.AddWithValue("@id", id);

            using SQLiteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Currency
                {
                    Id = Convert.ToInt32(reader["id"]),
                    Code = reader["code"].ToString(),
                    Name = reader["name"].ToString()
                };
            }

            return null;
        }
    }
}
