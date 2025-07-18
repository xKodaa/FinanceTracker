﻿using FinanceTracker.Model;
using FinanceTracker.Model.Repository;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System.Net;
using System.Windows;


namespace FinanceTracker.Graphics.Windows
{

    public partial class RegisterWindow : Window
    {
        private MainWindow MainWindow { get; set; }

        public RegisterWindow(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            this.Icon = mainWindow.Icon;
            InitializeComponent();
            InitializeCurrencyComboBox();
        }

        private void InitializeCurrencyComboBox()
        {

            CurrencyRepository currencyService = new();
            List<Currency> currencies = currencyService.GetAllCurrencies();

            RegisterCurrencyComboBox.ItemsSource = currencies;
            RegisterCurrencyComboBox.SelectedIndex = 0; // defaultně první měna
        }

        // Zpracování uživatelských hodnot po kliknutí na "registrovat se"
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterService registerService = new RegisterService();
            string name = FirstNameBox.Text;
            string surname = LastNameBox.Text;
            string username = RegisterUsernameBox.Text;
            string password = new NetworkCredential(string.Empty, RegisterPasswordBox.SecurePassword).Password;
            Currency selectedCurrency = RegisterCurrencyComboBox.SelectedItem as Currency;

            if (!Util.ValidLoginOrRegistrationInputs(name, surname, username, password))
            {
                ClearInputs();
            }
            else
            {
                if (registerService.Register(name, surname, username, password, selectedCurrency))
                {
                    Logger.WriteLog(this, $"Uživatel '{username}' zaregistrován");
                    User user = new(username)
                    {
                        Name = name,
                        Surname = surname,
                        LastLogin = DateTime.Now
                    };
                    UserInfoService.SetUser(user);
                    DialogResult = true;
                }
                else
                    ClearInputs();
            }
        }

        // Přesměrování na přihlašovací formulář přes hypertext
        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            LoginWindow loginWindow = new LoginWindow(MainWindow);
            loginWindow.ShowDialog();
        }

        private void ClearInputs()
        {
            RegisterPasswordBox.Clear();
            RegisterUsernameBox.Clear();
        }

        // Proti obejití "pokračování bez registrace"
        private void CloseApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null)
                Environment.Exit(0);
        }

        /* Možnost zobrazování a skrývání hesla */
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            RegisterPasswordTBox.Visibility = Visibility.Visible;
            RegisterPasswordBox.Visibility = Visibility.Hidden;
            RegisterPasswordTBox.Text = new NetworkCredential(string.Empty, RegisterPasswordBox.SecurePassword).Password;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            RegisterPasswordBox.Visibility = Visibility.Visible;
            RegisterPasswordTBox.Visibility = Visibility.Collapsed;
            RegisterPasswordBox.Password = RegisterPasswordTBox.Text;
        }
    }
}
