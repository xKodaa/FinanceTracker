using FinanceTracker.Graphics.Windows;
using FinanceTracker.Model.Services;

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
