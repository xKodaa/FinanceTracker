using FinanceTracker.Model;
using FinanceTracker.Model.Services;
using System;
using System.Collections.Generic;
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

        public CryptocurrenciesPage(MainWindow mainWindow)
        {
            cryptoCurrencies = new();
            InitializeComponent();
            LoadCryptoCurrencies();
        }

        private async void LoadCryptoCurrencies()
        {
            CryptoApiService CryptoApiService = new();
            cryptoCurrencies = await CryptoApiService.RetrieveCryptoInfoAsync();
            Dispatcher.Invoke(() =>
            {
                CryptoDataGrid.ItemsSource = cryptoCurrencies;
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
    }
}
