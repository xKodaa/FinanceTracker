using System.Data.SQLite;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Repository;
using FinanceTracker.Utility;

namespace FinanceTracker.Model.Services
{
    public class RegisterService
    {
        private readonly DatabaseConnector connector;
        private readonly SQLiteConnection connection;
        private readonly DatabaseContentService databaseContentService;

        public RegisterService()
        {
            connector = DatabaseConnector.Instance;
            connection = connector.Connection;
            databaseContentService = new();
        }

        // Zajišťuje uživatelskou registraci do databáze
        public bool Register(string name, string surname, string username, string password, Currency currency)
        {
            return new UserRepository().Register(name, surname, username, password, currency);
        }
    }
}
