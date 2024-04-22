using FinanceTracker.Graphics.Dialogs;
using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;


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
            LoadUserExpenses();
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
            if (decimal.TryParse(price.ToString(), out decimal amount))
            {
                string category = (string)FinancesCategoryComboBox.SelectedItem;
                string dateString = FinancesDatePicker.Text;
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    if (!Util.NonFutureDateTime(date))
                    {
                        Util.ShowErrorMessageBox("Nelze zadat budoucí datum");
                        return;
                    }
                    UserExpenses userExpenses = new(amount, category, date);
                    if (SaveExpenseIntoDatabase(userExpenses)) 
                    {
                        FinancesDataGrid.Items.Add(userExpenses);
                        ClearPage();
                    }
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
        private bool SaveExpenseIntoDatabase(UserExpenses userExpenses)
        {
            try
            {
                string sql = "INSERT INTO UserFinances VALUES (@username, @category, @date, @price)";
                using SQLiteCommand command = new(sql, Connector.Connection);
                User user = Connector.LoggedUser;
                string dateString = userExpenses.Date.ToString("yyyy-MM-dd");
                command.Parameters.AddWithValue("@username", user.Username);
                command.Parameters.AddWithValue("@category", userExpenses.Category);
                command.Parameters.AddWithValue("@date", dateString);
                command.Parameters.AddWithValue("@price", userExpenses.Price);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Util.ShowInfoMessageBox("Výdaj byl úspěšně uložen.");
                    return true;
                }
                else
                {
                    Util.ShowErrorMessageBox("Nepodařilo se uložit výdaj do databáze");
                    Logger.WriteErrorLog(nameof(FinancesPage), "Nepodařilo se uložit výdaj do databáze");
                }
            }
            catch (Exception ex) 
            {
                Util.ShowErrorMessageBox("Nepodařilo se uložit výdaj do databáze");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se uložit výdaj do databáze, {ex.Message}");
            }
            return false;
        }

        // Převod na stránku s kryptoměnami
        private void DashboardsHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(new DashboardPage(MainWindow));
        }

        // Přidá novou kategorii do ComboBoxu a do konfiguračního souboru a v comboboxu ji vybere
        private void FinancesBtnAddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryDialog addCategoryDialog = new(MainWindow);
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
            FinancesSpentTextBox.Clear();
        }

        private void LoadUserExpenses()
        {
            try
            {
                string sql = $"SELECT * FROM UserFinances WHERE username=@username";
                using SQLiteCommand command = new(sql, Connector.Connection);
                string username = Connector.LoggedUser.Username;
                command.Parameters.AddWithValue("@username", username);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string category = reader.GetString(1);
                    DateTime date = reader.GetDateTime(2);
                    decimal price = reader.GetDecimal(3);
                    if (category != null && price != 0)
                    {
                        UserExpenses userExpenses = new UserExpenses(price, category, date);
                        FinancesDataGrid.Items.Add(userExpenses);
                        Logger.WriteLog(nameof(FinancesPage), $"Načtené hodnoty z UserFinances: {userExpenses}");
                    }
                    else
                    {
                        Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se načíst hodnoty z UserFinances pro uživatele ${username}");
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se smazat kryptoměnu z databáze");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se smazat kryptoměnu z databáze, {ex.Message}");
            }
        }

        // Smaže vybraný záznam z tabulky a z databáze
        private void FinancesButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (FinancesDataGrid.SelectedItem == null) return;

            UserExpenses userExpenses = (UserExpenses)FinancesDataGrid.SelectedItem;
            if (DeleteUserExpenseFromDatabase(userExpenses))
                FinancesDataGrid.Items.Remove(userExpenses);
        }

        // Smaže všechny uživatelem vložené záznamy
        private void FinancesButtonDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql = $"DELETE FROM UserFinances";

                using SQLiteCommand command = new SQLiteCommand(sql, Connector.Connection);
                int rowsAffected = command.ExecuteNonQuery();
                FinancesDataGrid.Items.Clear();

                if (!(rowsAffected > 0))
                {
                    Util.ShowInfoMessageBox("Není co mazat.");
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se smazat všechny záznamy");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se smazat všechny záznamy finnancí z databáze, {ex.Message}");
            }
        }

        private bool DeleteUserExpenseFromDatabase(UserExpenses userExpenses)
        {
            try
            {
                string sql = $"DELETE FROM UserFinances WHERE username=@username AND category=@category AND price=@price AND date=@date";

                using SQLiteCommand command = new(sql, Connector.Connection);
                string username = Connector.LoggedUser.Username;
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@category", userExpenses.Category);
                command.Parameters.AddWithValue("@price", userExpenses.Price);
                command.Parameters.AddWithValue("@date", userExpenses.Date.ToString("yyyy-MM-dd"));
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Util.ShowInfoMessageBox("Záznam byl úspěšně smazán.");
                    return true;
                }
                else
                {
                    Util.ShowInfoMessageBox("Žádný záznam nebyl smazán. Zkontrolujte, zda záznam existuje.");
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se smazat záznam");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se smazat záznam finance z databáze, {ex.Message}");
            }
            return false;
        }
    }
}
