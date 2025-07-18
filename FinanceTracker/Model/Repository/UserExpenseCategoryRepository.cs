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
    class UserExpenseCategoryRepository
    {
        private static string TableName = "user_expense_categories";
        private readonly string[] Categories = [ "Potraviny", "Bydlení", "Zdravotní péče", "Doprava", "Vzdělání", "Zábava a volný čas",
        "Oblečení a osobní péče", "Děti a péče o rodinu", "Restaurace a stravování venku", "Spoření a investice" ];

        public void CreateCategoryForUser(string username)
        {
            string sql = $"INSERT INTO {TableName} (username, category) VALUES (@username, @category)";

            using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);

            foreach (var category in Categories)
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@category", category);
                int success = command.ExecuteNonQuery();
                if (success == 0)
                {
                    Logger.WriteErrorLog(this, $"Nepodařilo se vytvořit kategorii '{category}' pro uživatele '{username}'");
                    Util.ShowErrorMessageBox($"Nepodařilo se vytvořit kategorii '{category}'");
                }
                command.Parameters.Clear();
            }
        }

        public List<string> GetCategoriesForUser(string username)
        {
            List<string> categories = new();
            string sql = $"SELECT category FROM {TableName} WHERE username=@username GROUP BY category ORDER BY category";

            using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);
            command.Parameters.AddWithValue("@username", username);

            using SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(reader["category"].ToString());
            }

            return categories;
        }
        public bool AddCategory(string username, string categoryName)
        {
            if (!CategoryExists(username, categoryName))
            {
                string sql = $"INSERT INTO {TableName} (username, category) VALUES (@username, @category)";
                using SQLiteCommand cmd = new(sql, DatabaseConnector.Instance.Connection);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@category", categoryName);
                return cmd.ExecuteNonQuery() > 0;
            }
            return false;
        }

        public bool DeleteCategory(string username, string categoryName)
        {
            if (CategoryExists(username, categoryName))
            {
                string sql = $"DELETE FROM {TableName} WHERE username = @username AND category = @category";
                using SQLiteCommand command = new(sql, DatabaseConnector.Instance.Connection);
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@category", categoryName);
                return command.ExecuteNonQuery() > 0;
            }
            return false;
        }

        public bool CategoryExists(string username, string categoryName)
        {
            string sql = $"SELECT COUNT(*) FROM {TableName} WHERE username = @username AND category = @category";
            using SQLiteCommand cmd = new(sql, DatabaseConnector.Instance.Connection);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@category", categoryName);
            int exists = Convert.ToInt32(cmd.ExecuteScalar());
            return exists > 0;
        }
    }
}
