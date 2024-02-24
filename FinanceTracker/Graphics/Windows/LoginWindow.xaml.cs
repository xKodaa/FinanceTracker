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
                    Util.Util.ShowErrorMessageBox("Login unsuccessful, please try again");
                    ClearInputs();
                }
                else
                {
                    DialogResult = true;
                }
            }
        }

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

        private void CloseApp(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == null) 
                Environment.Exit(0);
        }
    }
}
