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
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        // Zpracování uživatelských hodnot po kliknutí na "registrovat se"
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterService registerService = new RegisterService();
            string username = RegisterUsernameBox.Text;
            string password = new NetworkCredential(string.Empty, RegisterPasswordBox.SecurePassword).Password;

            if (!Util.Util.ValidLoginOrRegistrationInputs(username, password))
            {
                ClearInputs();
            }
            else 
            {
                if (registerService.Register(username, password))
                {
                    DialogResult = true;
                    Util.Util.ShowInfoMessageBox("Registrace byla úspěšná");
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
    }
}
