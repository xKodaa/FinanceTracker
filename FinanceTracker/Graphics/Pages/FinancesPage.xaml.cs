using FinanceTracker.Graphics.Dialogs;
using FinanceTracker.Model;
using FinanceTracker.Utility;
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
        private AppConfig AppConfig { get; set; }

        public FinancesPage(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            Frame = MainWindow.MainContentFrame;
            AppConfig = Util.ReadAppConfig();
            InitializeComponent();
            InitCategoryComboBox();
            FinancesDatePicker.SelectedDate = DateTime.Now;
        }

        private void InitCategoryComboBox()
        {
            if (AppConfig == null || AppConfig.FinanceCategories == null)
            {
                Util.ShowErrorMessageBox("Nepovedlo se načít nákupní kategorie z konfiguračního souboru");
                Logger.WriteErrorLog(nameof(FinancesPage), "FinanceCategories nenačteny");
                Environment.Exit(0);
            }
            AppConfig.FinanceCategories.ForEach(category => FinancesComboBox.Items.Add(category));
            FinancesComboBox.SelectedIndex = 0;
        }

        private void FinancesBtnSubmit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DashboardsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new DashboardPage(MainWindow));
        }

        private void FinancesBtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryDialog addCategoryDialog = new AddCategoryDialog();
            if (addCategoryDialog.ShowDialog() == true)
            {
                string? categoryName = addCategoryDialog.CategoryName;
                if (categoryName != null && !AppConfig.FinanceCategories.Contains(categoryName))
                {
                    AppConfig.FinanceCategories.Add(categoryName);
                    Util.EditAppConfig("FinanceCategories", AppConfig.FinanceCategories);
                    FinancesComboBox.Items.Add(categoryName);
                }
            }
        }

        // Odebere aktuálně vybraný item z ComboBoxu a odebere ho i z konfiguračního souboru
        private void FinancesBtnRemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            object item = FinancesComboBox.SelectedItem;
            int index = FinancesComboBox.SelectedIndex;
            if (item != null && AppConfig.FinanceCategories != null)
            {
                FinancesComboBox.Items.RemoveAt(index);
                FinancesComboBox.SelectedIndex = 0;
                AppConfig.FinanceCategories.Remove(item.ToString());
                Util.EditAppConfig("FinanceCategories", AppConfig.FinanceCategories);
            }
        }
    }
}
