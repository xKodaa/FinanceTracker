using FinanceTracker.Graphics.Pages;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Repository
{
    class UserExpensesRepository
    {
        private static string TableName = "user_expenses";
        private static SQLiteConnection Connecion => DatabaseConnector.Instance.Connection;

        public DateTime GetLastUserExpenseDate(String username)
        {
            string sql = $"SELECT MAX(date) FROM {TableName} WHERE username=@username";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connecion))
            {
                command.Parameters.AddWithValue("@username", username);
                object res = command.ExecuteScalar();
                return Convert.ToDateTime(res);
            }
        }

        public int SaveUserExpensesForUser(UserExpenses userExpenses, string username)
        {
            string sql = $"INSERT INTO {TableName} VALUES (@username, @category, @date, @price)";
            using SQLiteCommand command = new(sql, Connecion);
            string dateString = userExpenses.Date.ToString("yyyy-MM-dd");
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@category", userExpenses.Category);
            command.Parameters.AddWithValue("@date", dateString);
            command.Parameters.AddWithValue("@price", userExpenses.Price);
            return command.ExecuteNonQuery();
        }

        public List<UserExpenses> GetUserExpenses(string username)
        {
            List<UserExpenses> expenses = new();

            string sql = $"SELECT category, date, price FROM {TableName} WHERE username=@username";
            using SQLiteCommand command = new(sql, Connecion);
            command.Parameters.AddWithValue("@username", username);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                string category = reader.GetString(0);
                DateTime date = reader.GetDateTime(1);
                decimal price = reader.GetDecimal(2);

                expenses.Add(new UserExpenses(price, category, date));
            }

            return expenses;
        }

        public bool DeleteExpense(string username, UserExpenses expense)
        {
            string sql = $"DELETE FROM {TableName} WHERE username=@username AND category=@category AND price=@price AND date=@date";
            using SQLiteCommand command = new(sql, Connecion);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@category", expense.Category);
            command.Parameters.AddWithValue("@price", expense.Price);
            command.Parameters.AddWithValue("@date", expense.Date.ToString("yyyy-MM-dd"));
            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteAllExpenses(string username)
        {
            string sql = $"DELETE FROM {TableName} WHERE username = @username";
            using SQLiteCommand command = new(sql, Connecion);
            command.Parameters.AddWithValue("@username", username);
            return command.ExecuteNonQuery() > 0;
        }

        public List<int> GetAvailableYearsForUser(string username)
        {
            List<int> years = [];
            string sql = @"SELECT DISTINCT strftime('%Y', date) AS Year FROM user_expenses WHERE username = @username ORDER BY Year DESC";

            using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);
            command.Parameters.AddWithValue("@username", username);

            using SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string? yearString = reader["Year"].ToString();
                if (int.TryParse(yearString, out int year))
                    years.Add(year);
            }

            return years;
        }

        public List<(string Category, double Total)> GetUserExpensesByCategory(string username, int year, int month, string mode)
        {
            List<(string Category, double Total)> result = [];
            string sql = "";

            if (mode == "monthly")
            {
                sql = @"SELECT category, SUM(price) as TotalPrice 
                FROM user_expenses 
                WHERE username = @username AND strftime('%Y', date) = @year AND strftime('%m', date) = @month
                GROUP BY category";
            }
            else if (mode == "quarterly")
            {
                int quarterEndMonth = month + 2;
                sql = @"SELECT category, SUM(price) as TotalPrice 
                FROM user_expenses 
                WHERE username = @username AND strftime('%Y', date) = @year 
                AND (strftime('%m', date) >= @startMonth AND strftime('%m', date) <= @endMonth)
                GROUP BY category";
            }
            else if (mode == "yearly")
            {
                sql = @"SELECT category, SUM(price) as TotalPrice 
                FROM user_expenses 
                WHERE username = @username AND strftime('%Y', date) = @year
                GROUP BY category";
            }

            using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);
            command.Parameters.AddWithValue("@username", username);
            command.Parameters.AddWithValue("@year", year.ToString());

            if (mode == "monthly")
            {
                command.Parameters.AddWithValue("@month", month.ToString("D2"));
            }
            else if (mode == "quarterly")
            {
                command.Parameters.AddWithValue("@startMonth", month.ToString("D2"));
                command.Parameters.AddWithValue("@endMonth", (month + 2).ToString("D2"));
            }

            using SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string? category = reader["category"].ToString();
                double total = Convert.ToDouble(reader["TotalPrice"]);
                if (category != null)
                    result.Add((category, total));
            }

            return result;
        }
    }
}
