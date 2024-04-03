using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;
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

        private void InitCryptoComponents()
        {
            // Všechny kryptoměny
            foreach (CryptoCurrency currency in cryptoCurrencies)
            {
                AllCryptoComboBox.Items.Add(currency);
            }
            AllCryptoComboBox.SelectedIndex = 0;


            // Uživatelské kryptoměny
            List<UserCryptoCurrency> userCryptoCurrencies = LoadUserCryptosFromDatabase();
            foreach (UserCryptoCurrency userCryptoCurrency in userCryptoCurrencies)
            {
                if (userCryptoCurrency.FindCryptoFromList(cryptoCurrencies))
                { 
                    UserCryptoDataGrid.Items.Add(userCryptoCurrency);
                }
            }
        }

        // Asynchronně načte kryptoměny z API a vloží je do tabulky
        private async void LoadCryptoCurrencies()
        {
            CryptoApiService CryptoApiService = new();
            cryptoCurrencies = await CryptoApiService.RetrieveCryptoInfoAsync();
            Dispatcher.Invoke(() =>
            {
                CryptoDataGrid.ItemsSource = cryptoCurrencies;
                InitCryptoComponents();
            });
        }

        // Filtruje kryptoměny podle zadaného textu
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

        // Obarví řádky tabulky podle změny ceny
        private void OnCryptoDataGridLoadingRow(object sender, DataGridRowEventArgs e)
        {
            // Všechny kryptoměny v tabulce
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
                return;
            }

            // Uživatelské kryptoměny v tabulce
            UserCryptoCurrency? userCurrency = e.Row.DataContext as UserCryptoCurrency;
            if (userCurrency != null) 
            {
                decimal difference = userCurrency.Difference;
                if (difference > 0)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.LimeGreen);
                }
                else
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Red);
                }
            }
        }

        // Přidání kryptoměny do databáze a do tabulky
        private void CryptoButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            CryptoCurrency userCrypto = (CryptoCurrency) AllCryptoComboBox.SelectedItem;
            string cryptoName = userCrypto.Symbol; 
            string dateString = CryptoPurchaseDatePicker.Text;
            if (!DateTime.TryParse(dateString, out DateTime dateOfBuy))
            {
                Util.ShowErrorMessageBox("Neplatný datum");
                return;
            }
            if (!decimal.TryParse(CryptoQuantityTextBox.Text, out decimal amount))
            {
                Util.ShowErrorMessageBox("Neplatné množství");
                return;
            }
            if (!decimal.TryParse(CryptoPurchasePriceTextBox.Text, out decimal price))
            {
                Util.ShowErrorMessageBox("Neplatná cena");
                return;
            }

            if (!Util.NonFutureDateTime(dateOfBuy))
            {
                Util.ShowErrorMessageBox("Nelze zadat budoucí datum");
                return;
            }

            // Přidání userCryptoCurrency do databáze
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
                        Util.ShowErrorMessageBox("Uložení dat se nezdařilo.");
                    }
                    else 
                    {
                        AddUserCryptoCurrency(userCryptoCurrency);
                        ClearPage();
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nastala chyba při vkládání dat");
                Logger.WriteErrorLog(nameof(CryptocurrenciesPage), $"Nastala chyba při vkládání dat, {ex.Message}");
            }
        }

        private void ClearPage()
        {
            CryptoPurchaseDatePicker.Text = "";
            CryptoQuantityTextBox.Text = "";
            CryptoPurchasePriceTextBox.Text = "";
        }

        // Přidání uživatelské kryptoměny do tabulky
        private void AddUserCryptoCurrency(UserCryptoCurrency userCrypto)
        {
            if (userCrypto.FindCryptoFromList(cryptoCurrencies)) 
            {
                UserCryptoDataGrid.Items.Add(userCrypto);
            }
        }

        // Smazání uživatelské kryptoměny z tabulky a databáze
        private void CryptoButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (UserCryptoDataGrid.SelectedItem == null) return;
            UserCryptoCurrency userSelectedCrypto = (UserCryptoCurrency)UserCryptoDataGrid.SelectedItem;
            RemoveCryptoFromDatabase(userSelectedCrypto);
            UserCryptoDataGrid.Items.Remove(userSelectedCrypto);    
        }

        private void RemoveCryptoFromDatabase(UserCryptoCurrency userSelectedCrypto)
        {
            try
            {
                string sql = $"DELETE FROM UserCryptos WHERE username=@username AND cryptoName=@cryptoName AND amount=@amount AND dateOfBuy=@dateOfBuy AND price=@price";
                using (SQLiteCommand command = new SQLiteCommand(sql, connector.Connection))
                {
                    string username = connector.LoggedUser.Username;
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@cryptoName", userSelectedCrypto.Symbol);
                    command.Parameters.AddWithValue("@amount", userSelectedCrypto.Amount);
                    command.Parameters.AddWithValue("@dateOfBuy", userSelectedCrypto.DateOfBuy);
                    command.Parameters.AddWithValue("@price", userSelectedCrypto.PricePerAmount);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se smazat kryptoměnu z databáze");
                Logger.WriteErrorLog(nameof(CryptocurrenciesPage), $"Nepodařilo se smazat kryptoměnu z databáze, {ex.Message}");
            }
        }

        // Načtení uživatelských kryptoměn z databáze
        private List<UserCryptoCurrency> LoadUserCryptosFromDatabase()
        {
            try
            {
                string sql = $"SELECT cryptoName, amount, dateOfBuy, price FROM UserCryptos WHERE username=@username";
                using (SQLiteCommand command = new SQLiteCommand(sql, connector.Connection))
                {
                    string username = connector.LoggedUser.Username;
                    command.Parameters.AddWithValue("@username", username);
                    List<UserCryptoCurrency> results = [];
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string cryptoName = reader.GetString(0);
                            decimal amount = reader.GetDecimal(1);
                            DateTime dateOfBuy = reader.GetDateTime(2);
                            decimal price = reader.GetDecimal(3);
                            if (cryptoName != null && amount != 0 && price != 0)
                            {
                                UserCryptoCurrency userCryptoCurrency = new UserCryptoCurrency(cryptoName, amount, price, dateOfBuy);
                                results.Add(userCryptoCurrency);
                                Logger.WriteLog(nameof(CryptocurrenciesPage), $"Načtené hodnoty z UserCryptos: {userCryptoCurrency}");
                            } else 
                            {
                                Logger.WriteErrorLog(nameof(CryptocurrenciesPage), $"Nepodařilo se načíst hodnoty z UserCryptos: {cryptoName}, {amount}, {dateOfBuy}, {price}");
                            }
                        }
                        return results;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se načíst kryptoměnu z databáze");
                Logger.WriteErrorLog(nameof(CryptocurrenciesPage), $"Nepodařilo se načíst kryptoměnu z databáze, {ex.Message}");
            }
            return [];
        }

        private void CryptoRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCryptoCurrencies();
        }
    }
}
