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

        public void InitUsersCategories(string username)
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
    }
}
