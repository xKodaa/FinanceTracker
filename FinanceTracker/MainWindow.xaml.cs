using FinanceTracker.Config;
using FinanceTracker.Graphics.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FinanceTracker.Utility;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using FinanceTracker.Model;


namespace FinanceTracker
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            /*CurrencyApiService currencyConvertorService = new CurrencyApiService();
            CryptoApiService cryptoApiService = new CryptoApiService();

            currencyConvertorService.ConvertCurrencyAsync("USD", "EUR");
            cryptoApiService.RetrieveCryptoInfoAsync();*/
            Util.EditAppConfig("DefaultCurrency", "USD");    
            //Launcher launcher = new(this);
            //launcher.Launch();
            
            //InitializeComponent();
        }

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ProfilePage()); 
        }
    }
}