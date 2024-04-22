using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System.Data.SQLite;
using System.Windows.Controls;

namespace FinanceTracker.Graphics.Pages
{
    public partial class ProfilePage : Page
    {
        private User LoggedUser { get; set; }
        private DatabaseConnector Connector { get; set; }

        public ProfilePage(MainWindow mainWindow)
        {
            LoggedUser = Util.LoadUser();
            Connector = DatabaseConnector.Instance;
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            SetBasicUserData();
            SetUserCryptoCount();
            SetUserFinancesDate();
        }

        // Nastavení základních uživatelských údajů
        private void SetBasicUserData()
        {
            UsernameLabel.Text = LoggedUser.Username;
            NameLabel.Text = LoggedUser.Name;
            SurnameLabel.Text = LoggedUser.Surname;
            LastLoginLabel.Text = LoggedUser.LastLogin?.ToString("dd.MM.yyyy");
        }

        // Nastavení počtu kryptoměn uživatele
        private void SetUserCryptoCount()
        {
            string sql = "SELECT COUNT(DISTINCT cryptoName) FROM UserCryptos WHERE username=@username";
            using SQLiteCommand command = new(sql, Connector.Connection);
            command.Parameters.AddWithValue("@username", LoggedUser.Username);
            object result = command.ExecuteScalar();
            try
            {
                int count = Convert.ToInt32(result);
                if (count < 0)
                {
                    Logger.WriteErrorLog(this, $"Uživateli '{LoggedUser.Username}' se načetl špatně počet kryptoměn");
                }
                else
                {
                    if (count == 0)
                    {
                        CryptoCountLabel.Text = "Žádné záznamy";
                    }
                    else
                    {
                        CryptoCountLabel.Text = count.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteErrorLog(this, $"Chyba při načítání počtu uživatelských kryptoměn: {e}");
            }
        }

        // Nastavení data posledního záznamu financí uživatele
        private void SetUserFinancesDate()
        {
            string sql = "SELECT MAX(date) FROM UserFinances WHERE username=@username";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connector.Connection))
            {
                command.Parameters.AddWithValue("@username", LoggedUser.Username);
                object result = command.ExecuteScalar();
                try 
                {
                    DateTime date = Convert.ToDateTime(result);
                    LastFinanceRecordLabel.Text = date.ToString("dd.MM.yyyy");
                } 
                catch (Exception e)
                {
                    LastFinanceRecordLabel.Text = "Žádné záznamy";
                    Logger.WriteErrorLog(this, $"Chyba při načítání posledního záznamu financí: {e}");
                }
            }
        }
    }
}
