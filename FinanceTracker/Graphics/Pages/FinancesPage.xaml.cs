using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceTracker.Graphics.Pages
{
    public partial class FinancesPage : Page
    {
        private Frame Frame { get; set; }
        private MainWindow MainWindow { get; set; }

        public FinancesPage(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            Frame = MainWindow.MainContentFrame;  
            InitializeComponent();
        }

        private void FinancesBtnSubmit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DashboardsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new DashboardPage(MainWindow));
        }
    }
}
