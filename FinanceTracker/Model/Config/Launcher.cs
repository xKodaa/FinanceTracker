using FinanceTracker.Graphics.Windows;
using FinanceTracker.Model.Services;

namespace FinanceTracker.Model.Config
{
    public class Launcher
    {
        private MainWindow MainWindow { get; set; }

        public Launcher(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
        }

        // Vytvoření databázového obsahu a následné spuštění aplikace
        public void Launch()
        {
            DatabaseContentService databaseContentService = new();
            databaseContentService.CheckDatabaseContent();
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
