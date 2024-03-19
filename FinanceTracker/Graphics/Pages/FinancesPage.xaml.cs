using FinanceTracker.Graphics.Dialogs;
using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Xml.Linq;

namespace FinanceTracker.Graphics.Pages
{
    public partial class FinancesPage : Page
    {
        private Frame Frame { get; set; }
        private MainWindow MainWindow { get; set; }
        private AppConfig AppConfig { get; set; }
        private readonly DatabaseConnector Connector;

        public FinancesPage(MainWindow mainWindow)
        {
            Connector = DatabaseConnector.Instance;
            MainWindow = mainWindow;
            Frame = MainWindow.MainContentFrame;
            AppConfig = Util.ReadAppConfig();
            InitializeComponent();
            InitCategoryComboBox();
            ClearPage();
        }

        // Naplnění comboboxů hodnotami z konfiguračního souboru
        private void InitCategoryComboBox()
        {
            if (AppConfig == null || AppConfig.FinanceCategories == null)
            {
                Util.ShowErrorMessageBox("Nepovedlo se načít nákupní kategorie z konfiguračního souboru");
                Logger.WriteErrorLog(nameof(FinancesPage), "FinanceCategories nenačteny");
                Environment.Exit(0);
            }
            AppConfig.FinanceCategories.ForEach(category => FinancesCategoryComboBox.Items.Add(category));
            FinancesCategoryComboBox.SelectedIndex = 0;
        }

        // Handler na tlačítko "Potvrdit"
        private void FinancesBtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (FinancesSpentTextBox.Text == "" || FinancesCategoryComboBox.SelectedItem == null || FinancesDatePicker.Text == null)
            {
                Util.ShowErrorMessageBox("Vyplňte prosím všechny údaje");
                Logger.WriteErrorLog(nameof(FinancesPage), "Uživatel nevyplnil všechny údaje");
                return;
            }
            string price = FinancesSpentTextBox.Text;
            if (int.TryParse(price.ToString(), out int amount))
            {
                string category = (string)FinancesCategoryComboBox.SelectedItem;
                string dateString = FinancesDatePicker.Text;
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    UserExpenses userExpenses = new UserExpenses(amount, category, date);
                    SaveExpenseIntoDatabase(userExpenses);
                    ClearPage();
                    Util.ShowInfoMessageBox("Výdaj byl úspěšně uložen");
                }
                else
                {
                    Util.ShowErrorMessageBox("Zadejte prosím datum ve správném formátu");
                    Logger.WriteErrorLog(nameof(FinancesPage), "Uživatel zadal datum v nesprávném formátu");
                }
            }
            else 
            {
                Util.ShowErrorMessageBox("Zadejte prosím číslo");
                Logger.WriteErrorLog(nameof(FinancesPage), "Uživatel do částky nezadal číslo");
            }
        }

        // Uložení výdaje do databáze
        private void SaveExpenseIntoDatabase(UserExpenses userExpenses)
        {
            try
            {
                string sql = "INSERT INTO UserFinances VALUES (@username, @category, @date, @price)";
                using (SQLiteCommand command = new SQLiteCommand(sql, Connector.Connection))
                {
                    User user = Connector.LoggedUser;
                    string dateString = userExpenses.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@category", userExpenses.Category);
                    command.Parameters.AddWithValue("@date", dateString);
                    command.Parameters.AddWithValue("@price", userExpenses.Price);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex) 
            {
                Util.ShowErrorMessageBox("Nepodařilo se uložit výdaj do databáze");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se uložit výdaj do databáze, {ex.Message}");
            }
        }

        // Převod na stránku s kryptoměnami
        private void DashboardsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new DashboardPage(MainWindow));
        }

        // Přidá novou kategorii do ComboBoxu a do konfiguračního souboru a v comboboxu ji vybere
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
                    FinancesCategoryComboBox.Items.Add(categoryName);
                    FinancesCategoryComboBox.SelectedIndex = FinancesCategoryComboBox.Items.Count - 1;
                }
            }
        }

        // Odebere aktuálně vybraný item z ComboBoxu a odebere ho i z konfiguračního souboru
        private void FinancesBtnRemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            object item = FinancesCategoryComboBox.SelectedItem;
            int index = FinancesCategoryComboBox.SelectedIndex;
            if (item != null && AppConfig.FinanceCategories != null)
            {
                FinancesCategoryComboBox.Items.RemoveAt(index);
                FinancesCategoryComboBox.SelectedIndex = 0;
                AppConfig.FinanceCategories.Remove(item.ToString());
                Util.EditAppConfig("FinanceCategories", AppConfig.FinanceCategories);
            }
        }
        private void ClearPage()
        {
            FinancesCategoryComboBox.SelectedIndex = 0;
            FinancesDatePicker.SelectedDate = DateTime.Now;
            FinancesSpentTextBox.Clear();
        }
    }
}
