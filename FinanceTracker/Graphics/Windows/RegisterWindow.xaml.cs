using FinanceTracker.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinanceTracker.Graphics.Windows
{

    public partial class RegisterWindow : Window
    {
        private MainWindow MainWindow { get; set; }

        public RegisterWindow(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
        }

        // Zpracování uživatelských hodnot po kliknutí na "registrovat se"
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterService registerService = new RegisterService();
            string name = FirstNameBox.Text;
            string surname = LastNameBox.Text;
            string username = RegisterUsernameBox.Text;
            string password = new NetworkCredential(string.Empty, RegisterPasswordBox.SecurePassword).Password;

            if (!Util.Util.ValidLoginOrRegistrationInputs(name, surname, username, password))
            {
                ClearInputs();
            }
            else
            {
                if (registerService.Register(name, surname, username, password))
                {
                    DialogResult = true;
                    // TODO toto místo to alertu přesunout spíše na uvodni profile page
                    Util.Util.ShowInfoMessageBox($"Registrace byla úspěšná! \n - Vítejte {name} {surname}");
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
        }
    }
}
