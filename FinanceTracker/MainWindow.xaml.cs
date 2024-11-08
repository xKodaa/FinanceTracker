﻿using FinanceTracker.Graphics.Pages;
using System.Windows;
using FinanceTracker.Utility;
using FinanceTracker.Model.Config;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace FinanceTracker
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            SetIcon();
            ResizeWindow();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Launcher launcher = new(this);
            launcher.Launch();
            UpdateMainTitle();
        }

        // Nastaví velikost okna na 95% obrazovky
        private void ResizeWindow()
        {
            double padding = 0.95;
            this.Width = SystemParameters.PrimaryScreenWidth * padding;
            this.Height = SystemParameters.PrimaryScreenHeight * padding;
        }

        private void SetIcon()
        {
            Uri iconUri = new("Data/icon.ico", UriKind.Relative);
            this.Icon = BitmapFrame.Create(iconUri);
        }

        // Přidá k názvu aplikace username uživatele
        private void UpdateMainTitle()
        {
            Title = $"Finance Tracker | {Util.GetUser().Username}";
        }

        /* Menu item handlers */
        private void MainPageLoaded(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ProfilePage(this)); 
        }

        private void FinanceButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new FinancesPage(this));
        }

        private void ConvertorButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ConvertorPage(this));
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
    }
}