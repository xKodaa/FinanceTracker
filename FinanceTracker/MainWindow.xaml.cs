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
            Launcher launcher = new(this);
            launcher.Launch();
            InitializeComponent();
            UpdateMainTitle();
        }

        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ProfilePage(this)); 
        }

        private void FinanceButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new FinancesPage(this));
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ProfilePage(this));
        }

        private void CryptoButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new CryptocurrenciesPage(this));

        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new DashboardPage(this));

        }

        private void UpdateMainTitle()
        {
            Title = $"Finance Tracker | {Util.GetUser()}";
        }
    }
}