using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Repository;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace FinanceTracker.Graphics.Pages
{
    public partial class CryptocurrenciesPage : Page
    {
        private List<CryptoCurrency> cryptoCurrencies;
        private readonly DatabaseConnector connector;
        private DispatcherTimer mainTimer;
        private DispatcherTimer updateLabelTimer;
        private DateTime nextTickTime;
        private readonly int refreshRate;
        private string labelContent;
        private UserCryptoRepository userCryptoRepository;

        public CryptocurrenciesPage(MainWindow mainWindow)
        {
            InitializeComponent();
            userCryptoRepository = new();
            mainTimer = new();
            updateLabelTimer = new();
            AppConfig appConfig = Util.ReadAppConfig();
            refreshRate = appConfig.CryptoRefreshRate;
            labelContent = $"Příští aktualizace za: {refreshRate}s";
            RunTimers();
            cryptoCurrencies = [];
            connector = DatabaseConnector.Instance;
            LoadCryptoCurrencies();
        }

        // Spustí časovače
        private void RunTimers()
        {
            mainTimer.Interval = TimeSpan.FromSeconds(refreshRate);
            LabelCountdown.Content = $"Příští aktualizace za: {refreshRate}s";
            mainTimer.Tick += MainTimer_Tick;
            mainTimer.Start();
            nextTickTime = DateTime.Now.Add(mainTimer.Interval);

            updateLabelTimer.Interval = TimeSpan.FromSeconds(1);
            updateLabelTimer.Tick += UpdateLabelTimer_Tick;
            updateLabelTimer.Start();
        }

        // Každou vteřinu aktualizuje label s časem do další aktualizace dat z API
        private void UpdateLabelTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan remainingTime = nextTickTime - DateTime.Now;
            Dispatcher.Invoke(() =>
            {
                LabelCountdown.Content = $"Příští aktualizace za: {remainingTime.Seconds}s";
            });
        }

        // Jednou za časový interval získaný z konfiguračního souboru načte kryptoměny z API
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            LoadCryptoCurrencies();
            nextTickTime = DateTime.Now.Add(mainTimer.Interval + TimeSpan.FromSeconds(1));
        }

        // Aktualizuje interval hlavního časovače => po kliknutí na tlačítko Obnovit 
        private void RefreshMainTimer()
        {
            mainTimer.Stop();
            mainTimer.Interval = TimeSpan.FromSeconds(refreshRate);
            mainTimer.Start();
            nextTickTime = DateTime.Now.Add(mainTimer.Interval);

            Dispatcher.Invoke(() =>
            {
                LabelCountdown.Content = $"Příští aktualizace za: {refreshRate}s";
            });
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
            UserCryptoDataGrid.Items.Clear();
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
            if (e.Row.DataContext is CryptoCurrency currency)
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
            if (e.Row.DataContext is UserCryptoCurrency userCurrency)
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
            CryptoCurrency userCrypto = (CryptoCurrency)AllCryptoComboBox.SelectedItem;
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
                Util.ShowErrorMessageBox($"Nelze zadat budoucí datum\nAktuální datum: {DateTime.Now:dd.MM.yyyy}");
                return;
            }

            if (amount <= 0)
            {
                Util.ShowErrorMessageBox("Množství musí být větší než 0");
                return;
            }

            if (price <= 0)
            {
                Util.ShowErrorMessageBox("Cena musí být větší než 0");
                return;
            }

            // Přidání userCryptoCurrency do databáze
            UserCryptoCurrency userCryptoCurrency = new(cryptoName, amount, price, dateOfBuy);
            if (userCryptoRepository.AddUserCrypto(userCryptoCurrency, connector.LoggedUser.Username))
            {
                AddUserCryptoCurrency(userCryptoCurrency);
                ClearPage();
            }
            else
            {
                Util.ShowErrorMessageBox("Uložení dat se nezdařilo.");
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
            userCryptoRepository.DeleteUserCrypto(userSelectedCrypto, connector.LoggedUser.Username);
        }

        // Načtení uživatelských kryptoměn z databáze
        private List<UserCryptoCurrency> LoadUserCryptosFromDatabase()
        {
            return userCryptoRepository.GetUserCryptos(connector.LoggedUser.Username);
        }

        private void CryptoRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCryptoCurrencies();
            RefreshMainTimer();
        }
    }
}
