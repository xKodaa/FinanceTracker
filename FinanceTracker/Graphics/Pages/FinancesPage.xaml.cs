using FinanceTracker.Graphics.Dialogs;
using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Repository;
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
        private UserExpensesRepository userExpensesRepository;
        private readonly UserExpenseCategoryRepository userExpenseCategoryRepository;
        private CurrencyRepository currencyRepository;

        public FinancesPage(MainWindow mainWindow)
        {
            Connector = DatabaseConnector.Instance;
            MainWindow = mainWindow;
            Frame = MainWindow.MainContentFrame;
            userExpensesRepository = new();
            userExpenseCategoryRepository = new();
            currencyRepository = new();
            AppConfig = Util.ReadAppConfig();
            InitializeComponent();
            InitializeCurrencyCombobox();
            InitCategoryComboBox();
            ClearPage();
            LoadUserExpenses();
        }

        private void InitializeCurrencyCombobox()
        {
            var currencies = currencyRepository.GetAllCurrencies();
            FinancesCurrencyComboBox.ItemsSource = currencies;
            FinancesCurrencyComboBox.DisplayMemberPath = "Code"; // ← zobrazí jen kód měny

            Currency userPreferredCurrency = currencyRepository.GetUserPreferredCurrency(Connector.LoggedUser.Username);
            if (userPreferredCurrency != null)
            {
                FinancesCurrencyComboBox.SelectedItem = currencies.FirstOrDefault(c => c.Id == userPreferredCurrency.Id);
            }
        }


        // Naplnění comboboxů kategorií hodnotami z databáze
        private void InitCategoryComboBox()
        {
            FinancesCategoryComboBox.Items.Clear();
            try
            {
                string username = Connector.LoggedUser.Username;
                var categories = userExpenseCategoryRepository.GetCategoriesForUser(username);

                if (categories.Count == 0)
                {
                    Util.ShowErrorMessageBox("Nemáte evidované žádné kategorie");
                    Logger.WriteErrorLog(nameof(FinancesPage), "FinanceCategories nenačteny z databáze");
                    return;
                }

                foreach (var category in categories)
                {
                    FinancesCategoryComboBox.Items.Add(category);
                }

                FinancesCategoryComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se načíst kategorie");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Chyba při načítání kategorií: {ex.Message}");
            }
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
                Currency currency = (Currency)FinancesCurrencyComboBox.SelectedItem;
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    if (!Util.NonFutureDateTime(date))
                    {
                        Util.ShowErrorMessageBox($"Nelze zadat budoucí datum\nAktuální datum: {DateTime.Now:dd.MM.yyyy}");
                        return;
                    }
                    UserExpenses userExpenses = new(amount, category, date, currency);
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
            User user = Connector.LoggedUser;
            int rowsAffected = userExpensesRepository.SaveUserExpensesForUser(userExpenses, user.Username);
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
                string categoryName = addCategoryDialog.CategoryName;

                bool added = AddCategoryIntoDatabase(categoryName);
                if (added)
                {
                    FinancesCategoryComboBox.Items.Add(categoryName);
                    FinancesCategoryComboBox.SelectedIndex = FinancesCategoryComboBox.Items.Count - 1;
                }
                else
                {
                    Util.ShowErrorMessageBox("Nepodařilo se přidat kategorii");
                    Logger.WriteErrorLog(nameof(FinancesPage), $"Nepodařilo se přidat kategorii {categoryName} do databáze");
                }
            }
        }

        // Přidání kategorie do databáze
        private bool AddCategoryIntoDatabase(string categoryName)
        {
            string username = Connector.LoggedUser.Username;
            if (userExpenseCategoryRepository.AddCategory(username, categoryName))
            {
                Util.ShowInfoMessageBox($"Kategorie {categoryName} byla úspěšně přidána");
                return true;
            }

            Util.ShowErrorMessageBox("Kategorie již existuje nebo se ji nepodařilo přidat");
            return false;
        }


        // Odebere aktuálně vybraný item z ComboBoxu a odebere ho i z konfiguračního souboru
        private void FinancesBtnRemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            string item = (string)FinancesCategoryComboBox.SelectedItem;
            int index = FinancesCategoryComboBox.SelectedIndex;
            if (item != null)
            {
                if (DeleteCategoryFromDatabase(item))
                {
                    FinancesCategoryComboBox.Items.RemoveAt(index);
                    FinancesCategoryComboBox.SelectedIndex = 0;
                }
            }
        }

        // Odebrání kategorie z databáze
        private bool DeleteCategoryFromDatabase(string categoryName)
        {
            string username = Connector.LoggedUser.Username;
            if (userExpenseCategoryRepository.DeleteCategory(username, categoryName))
            {
                Util.ShowInfoMessageBox($"Kategorie {categoryName} byla úspěšně odebrána");
                return true;
            }

            Util.ShowErrorMessageBox("Kategorie neexistuje nebo se ji nepodařilo odebrat");
            return false;
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
                string username = Connector.LoggedUser.Username;
                var expenses = userExpensesRepository.GetUserExpenses(username);

                foreach (var expense in expenses)
                {
                    FinancesDataGrid.Items.Add(expense);
                    Logger.WriteLog(nameof(FinancesPage), $"Načtené hodnoty z user_expenses: {expense}");
                }
            }
            catch (Exception ex)
            {
                Util.ShowErrorMessageBox("Nepodařilo se načíst výdaje z databáze");
                Logger.WriteErrorLog(nameof(FinancesPage), $"Chyba při načítání výdajů: {ex.Message}");
            }
        }

        // Smaže vybraný záznam z tabulky a z databáze
        private void FinancesButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (FinancesDataGrid.SelectedItem == null) return;

            string username = Connector.LoggedUser.Username;
            UserExpenses userExpenses = (UserExpenses)FinancesDataGrid.SelectedItem;
            if (userExpensesRepository.DeleteExpense(username, userExpenses))
                FinancesDataGrid.Items.Remove(userExpenses);
        }

        // Smaže všechny uživatelem vložené záznamy
        private void FinancesButtonDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            string username = Connector.LoggedUser.Username;
            if (userExpensesRepository.DeleteAllExpenses(username))
            {
                Util.ShowInfoMessageBox("Všechny záznamy byly úspěšně smazány.");
                FinancesDataGrid.Items.Clear();
            }
            else
            {
                Util.ShowInfoMessageBox("Nebyly nalezeny žádné záznamy k odstranění.");
            }
        }
    }
}
