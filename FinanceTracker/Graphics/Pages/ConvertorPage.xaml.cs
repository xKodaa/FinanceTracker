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
    /// <summary>
    /// Interakční logika pro ConvertorPage.xaml
    /// </summary>
    public partial class ConvertorPage : Page
    {
        public ConvertorPage(MainWindow mainWindow)
        {
            InitializeComponent();
            HideResultGrid();
        }

        private void ConvertorBtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ConversionResultLabel.Content.ToString());
        }

        private void ConvertorBtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ShowResultGrid();
        }

        private void HideResultGrid()
        {
            ConvertorResultGrid.Visibility = Visibility.Hidden;
        }

        private void ShowResultGrid()
        {
            ConvertorResultGrid.Visibility = Visibility.Visible;
        }
    }
}
