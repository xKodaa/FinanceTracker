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
    public partial class LoginWindow : Window
    {
        private MainWindow MainWindow { get; set; }

        public LoginWindow(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        // Zpracování uživatelských hodnot po kliknutí na "přihlásit se"
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginService loginService = new LoginService();
            string username = LoginUsernameBox.Text;
            string password = new NetworkCredential(string.Empty, LoginPasswordBox.SecurePassword).Password;

            if (!Util.Util.ValidLoginOrRegistrationInputs(username, password))
            {
                ClearInputs();
            }
            else 
            {
                bool loggedIn = loginService.Login(username, password);
                if (!loggedIn)
                {
                    Util.Util.ShowErrorMessageBox("Přihlášení se nepodařilo, prosím, zkuste to znovu");
                    ClearInputs();
                }
                else
                {
                    DialogResult = true;
                }
            }
        }

        // Přesměrování na registrační formulář přes hypertext
        private void RegisterHyperlink_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            RegisterWindow registerWindow = new RegisterWindow(MainWindow);
            Close();
            if (registerWindow.ShowDialog() == true)
            {
                registerWindow.Close();
            }
        }

        private void ClearInputs()
        {
            LoginPasswordBox.Clear();
            LoginUsernameBox.Clear();
        }

        // Proti obejití "pokračování bez přihlášení"
        private void CloseApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null) 
                Environment.Exit(0);
        }
    }
}
