using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model.Services
{
    class UserInfoService
    {
        private static SQLiteConnection Connection;
        private static DatabaseConnector Connector;

        static UserInfoService() 
        {
            Connector = DatabaseConnector.Instance;
            Connection = Connector.Connection;
        }

        // Nastavení uživatele singleton connectoru
        public static void SetUser(User user)
        {
            Connector = DatabaseConnector.Instance;
            Connector.LoggedUser = user;
            Logger.WriteLog(nameof(Util), $"Uživatel byl úspěšně nastaven '{user}'");
        }

        public static User GetUser()
        {
            return Connector.LoggedUser;
        }
    }
}
