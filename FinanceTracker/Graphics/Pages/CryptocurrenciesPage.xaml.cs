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

        public CryptocurrenciesPage(MainWindow mainWindow)
        {
            InitializeComponent();
            LoadCryptoCurrencies();
        }

        private async void LoadCryptoCurrencies()
        {
            CryptoApiService CryptoApiService = new();
            List<CryptoCurrency> cryptoCurrencies = await CryptoApiService.RetrieveCryptoInfoAsync();
            Dispatcher.Invoke(() =>
            {
                CryptoDataGrid.ItemsSource = cryptoCurrencies;
            });
        }

        private void FilterCryptoData(object sender, TextChangedEventArgs e)
        {

        }

        private void CryptoRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCryptoCurrencies();
        }
    }
}
