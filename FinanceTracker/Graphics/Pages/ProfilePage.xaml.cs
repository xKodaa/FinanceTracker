using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Repository;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System.Data.Entity.Core.Mapping;
using System.Data.SQLite;
using System.Windows.Controls;

namespace FinanceTracker.Graphics.Pages
{
    public partial class ProfilePage : Page
    {
        private User LoggedUser { get; set; }
        private DatabaseConnector Connector { get; set; }
        private UserExpensesRepository userExpenseRepository;
        private UserRepository userRepository;

        public ProfilePage(MainWindow mainWindow)
        {
            userRepository = new();
            userExpenseRepository = new();
            LoggedUser = userRepository.LoadUser();
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
            PreferedCurrency.Text = $"{LoggedUser.Currency.Name} ({LoggedUser.Currency.Code})";
        }

        // Nastavení počtu kryptoměn uživatele
        private void SetUserCryptoCount()
        {
            try
            {
                int count = new UserCryptoRepository().GetUsersNumberOfCryptos(LoggedUser.Username);
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
            try
            {
                DateTime date = userExpenseRepository.GetLastUserExpenseDate(LoggedUser.Username);
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
