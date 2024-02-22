using FinanceTracker.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Config
{
    public class Launcher
    {
        private DatabaseConnector DatabaseConnector { get; set; }
        private GraphTest GraphTest { get; set; }
        private MainWindow MainWindow { get; set; }

        public Launcher(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            DatabaseConnector = new();
            GraphTest = new(MainWindow);
        }

        public void Launch()
        {
            Console.WriteLine("FinanceTracker is running...");
        }
    }
}
