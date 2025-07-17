using FinanceTracker.Model.Config;
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
        public DateTime GetLastUserExpenseDate(String username)
        {
            string sql = "SELECT MAX(date) FROM UserFinances WHERE username=@username";
            using (SQLiteCommand command = new SQLiteCommand(sql, DatabaseConnector.Instance.Connection))
            {
                command.Parameters.AddWithValue("@username", username);
                object res = command.ExecuteScalar();
                return Convert.ToDateTime(res);
            }
        }
    }
}
