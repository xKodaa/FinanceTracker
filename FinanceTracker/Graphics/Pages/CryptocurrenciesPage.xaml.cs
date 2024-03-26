using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace FinanceTracker.Graphics.Pages
{
    public partial class CryptocurrenciesPage : Page
    {
        private List<CryptoCurrency> cryptoCurrencies;
        private readonly DatabaseConnector connector;

        public CryptocurrenciesPage(MainWindow mainWindow)
        {
            cryptoCurrencies = [];
            connector = DatabaseConnector.Instance;
            InitializeComponent();
            LoadCryptoCurrencies();
        }

        private void InitPersonalComboBoxes()
        {
            // Všechny kryptoměny
            foreach (CryptoCurrency currency in cryptoCurrencies)
            {
                AllCryptoComboBox.Items.Add(currency.Symbol);
            }
            AllCryptoComboBox.SelectedIndex = 0;


            // Uživatelské kryptoměny
            List<string> userCryptoCurrencies = LoadUserCryptosFromDatabase();
            foreach (string userCryptoCurrency in userCryptoCurrencies)
            {
                UserCryptoComboBox.Items.Add(userCryptoCurrency);
            }
            UserCryptoComboBox.SelectedIndex = 0;
        }


        private async void LoadCryptoCurrencies()
        {
            CryptoApiService CryptoApiService = new();
            cryptoCurrencies = await CryptoApiService.RetrieveCryptoInfoAsync();
            Dispatcher.Invoke(() =>
            {
                CryptoDataGrid.ItemsSource = cryptoCurrencies;
                InitPersonalComboBoxes();
            });
        }

        private void FilterCryptoData(object sender, TextChangedEventArgs e)
        {
            if (cryptoCurrencies == null)
            {
                return;
            }
            string filterText = CryptoFilterBox.Text;
            List<CryptoCurrency> filteredData = cryptoCurrencies.Where(crypto =>
                crypto.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                crypto.Symbol.Contains(filterText, StringComparison.OrdinalIgnoreCase) ||
                crypto.Rank.Contains(filterText, StringComparison.OrdinalIgnoreCase)).ToList();

            CryptoDataGrid.ItemsSource = filteredData;
        }

        private void CryptoRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCryptoCurrencies();
        }

        private void OnCryptoDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            CryptoCurrency? currency = e.Row.DataContext as CryptoCurrency;
            if (currency != null)
            {
                decimal changePercentage = currency.ChangePercent24HrDecimal;
                if (changePercentage > 0)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.LimeGreen);
                }
                else
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Red);
                }
      
            }
        }
        private void CryptoButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            string? cryptoName = AllCryptoComboBox.SelectedItem.ToString();
            string dateString = PurchaseDatePicker.Text;
            if (!DateTime.TryParse(dateString, out DateTime dateOfBuy))
            {
                MessageBox.Show("Neplatný datum");
                return;
            }
            if (!decimal.TryParse(QuantityTextBox.Text, out decimal amount))
            {
                MessageBox.Show("Neplatné množství");
                return;
            }
            if (!decimal.TryParse(PurchasePriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Neplatná cena");
                return;
            }

            if (cryptoName == null)
            {
                MessageBox.Show("Vyplňte prosím všechny údaje");
                return;
            }

            UserCryptoCurrency userCryptoCurrency = new UserCryptoCurrency(cryptoName, amount, price, dateOfBuy);
            
            try
            {
                string sql = "INSERT INTO UserCryptos (username, cryptoName, amount, dateOfBuy, price) VALUES (@username, @cryptoName, @amount, @dateOfBuy, @price)";

                using (SQLiteCommand command = new SQLiteCommand(sql, connector.Connection))
                {
                    string username = connector.LoggedUser.Username;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@cryptoName", cryptoName);
                    command.Parameters.AddWithValue("@amount", amount);
                    command.Parameters.AddWithValue("@dateOfBuy", dateOfBuy);
                    command.Parameters.AddWithValue("@price", price);

                    int result = command.ExecuteNonQuery();

                    if (!(result > 0))
                    {
                        MessageBox.Show("Vložení dat se nezdařilo.");
                    }
                    UserCryptoComboBox.Items.Add(cryptoName);
                    UserCryptoComboBox.SelectedIndex = 0;
                    AddUserCryptoIntoDataGrid(userCryptoCurrency);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nastala chyba při vkládání dat: {ex.Message}");
            }
        }

        private void AddUserCryptoIntoDataGrid(UserCryptoCurrency userCryptoCurrency)
        {
            UserCryptoDataGrid.Items.Add(userCryptoCurrency);
        }

        private void CryptoButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (UserCryptoComboBox.SelectedItem == null)
            {
                return;
            }
            UserCryptoComboBox.Items.Remove(UserCryptoComboBox.SelectedItem);
            string? userSelectedCrypto = UserCryptoComboBox.SelectedItem.ToString();
            if (userSelectedCrypto != null) { RemoveCryptoFromDatabase(userSelectedCrypto); }
        }

        private void RemoveCryptoFromDatabase(string userSelectedCrypto)
        {
            try
            {
                string sql = $"DELETE FROM UserCryptos WHERE username=@username & cryptoName=@cryptoName";
                using (SQLiteCommand command = new SQLiteCommand(sql, connector.Connection))
                {
                    string username = connector.LoggedUser.Username;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@cryptoName", userSelectedCrypto);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se smazat kryptoměnu z databáze");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se smazat kryptoměnu z databáze, {ex.Message}");
            }
        }

        private List<string> LoadUserCryptosFromDatabase()
        {
            try
            {
                string sql = $"SELECT cryptoName FROM UserCryptos WHERE username=@username";
                using (SQLiteCommand command = new SQLiteCommand(sql, connector.Connection))
                {
                    string username = connector.LoggedUser.Username;
                    command.Parameters.AddWithValue("@username", username);
                    List<string> results = [];
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string cryptoName = reader.GetString(0);
                            results.Add(cryptoName);
                        }
                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se uložit kryptoměnu do databáze");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se uložit kryptoměnu do databáze, {ex.Message}");
            }
            return [];
        }

  
    }
}
