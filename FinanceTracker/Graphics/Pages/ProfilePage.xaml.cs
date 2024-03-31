using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

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
        private void SetBasicUserData()
        {
            UsernameLabel.Text = LoggedUser.Username;
            NameLabel.Text = LoggedUser.Name;
            SurnameLabel.Text = LoggedUser.Surname;
            LastLoginLabel.Text = LoggedUser.LastLogin?.ToString("dd.MM.yyyy");
        }
        private void SetUserCryptoCount()
        {
            string sql = "SELECT COUNT(cryptoName) FROM UserCryptos WHERE username=@username";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connector.Connection))
            {
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
        }

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
