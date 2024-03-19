using FinanceTracker.Graphics;
using FinanceTracker.Graphics.Windows;
using FinanceTracker.Model;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FinanceTracker.Model.Config
{
    public class Launcher
    {
        private DatabaseConnector DatabaseConnector { get; set; }
        private MainWindow MainWindow { get; set; }


        public Launcher(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            DatabaseConnector = DatabaseConnector.Instance;
        }

        public void Launch()
        {
            new DatabaseContentService();
            HandleLoginWindow();
        }

        // Zajištění přihlášení/registrace uživatele
        private void HandleLoginWindow()
        {
            LoginWindow loginWindow = new(MainWindow);
            if (loginWindow.ShowDialog() == true)
            {
                loginWindow.Close();
            }
        }
    }
}
