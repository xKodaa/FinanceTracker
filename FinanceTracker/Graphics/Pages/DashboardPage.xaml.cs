﻿using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Model.Repository;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using LiveCharts;
using LiveCharts.Wpf;
using System.Data.SQLite;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;


namespace FinanceTracker.Graphics.Pages
{
    public partial class DashboardPage : Page
    {
        private MainWindow MainWindow { get; set; }
        public SeriesCollection MonthlyPieSeriesCollection { get; set; }
        public SeriesCollection MonthlyCartSeriesCollection { get; set; }
        public SeriesCollection QuartCartSeriesCollection { get; set; }
        public SeriesCollection QuartPieSeriesCollection { get; set; }
        public SeriesCollection YearlyCartSeriesCollection { get; set; }
        public SeriesCollection YearlyPieSeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private List<(string Category, double Total)> FinancesPerCategory;
        private DatabaseConnector Connector { get; set; }
        private User LoggedUser { get; set; }
        private readonly string GRAPH_TYPE_CARTESIAN = "Sloupcový";
        private readonly string GRAPH_TYPE_PIE = "Koláčový";
        private readonly string TAB_ITEM_MONTHLY_OVERVIEW = "Měsíční přehled";
        private readonly string TAB_ITEM_QUART_OVERVIEW = "Kvartální přehled";
        private readonly string TAB_ITEM_YEARLY_OVERVIEW = "Roční přehled";
        private bool justInitialized;
        private static UserExpensesRepository userExpensesRepository;

        public DashboardPage(MainWindow mainWindow)
        {
            InitializeComponent();
            userExpensesRepository = new();
            MainWindow = mainWindow;
            Connector = DatabaseConnector.Instance;
            LoggedUser = UserInfoService.GetUser();
            bool userHasSomeData = InitializeComboBoxes();
            MonthlyPieSeriesCollection = [];
            MonthlyCartSeriesCollection = [];
            QuartCartSeriesCollection = [];
            QuartPieSeriesCollection = [];
            YearlyCartSeriesCollection = [];
            YearlyPieSeriesCollection = [];
            Labels = [];
            FinancesPerCategory = [];
            Formatter = value => value.ToString("N");
            DateTime now = DateTime.Now;
            justInitialized = true;
            if (userHasSomeData) 
                RefreshGraph(now.Year, now.Month, GRAPH_TYPE_PIE, TAB_ITEM_MONTHLY_OVERVIEW);  // Zobrazení grafu pro aktuální měsíc
        }

        // Obnovení grafu pro zvolený rok a měsíc a zvolený typ grafu
        private void RefreshGraph(int year, int month, string selectedGraphType, string selectedTabItem)
        {
            FinancesPerCategory = LoadFinanceDataByCategory(LoggedUser.Username, year, month, selectedTabItem);
            if (FinancesPerCategory.Count == 0 && !justInitialized)
                Util.ShowInfoMessageBox("Za vybrané období nemáte evidované žádné záznamy.");
            justInitialized = false;
            DisplayFinanceData(selectedGraphType, selectedTabItem);
            MainWindow.DataContext = this;
        }

        // Zobrazení načtených dat z databáze na základě zvoleného typu grafu a záložky
        private void DisplayFinanceData(string selectedGraphType, string selectedTabItem)
        {
            if (selectedGraphType == null) return;

            if (selectedTabItem.Equals(TAB_ITEM_MONTHLY_OVERVIEW))
            {
                if (selectedGraphType.Equals(GRAPH_TYPE_CARTESIAN))
                {
                    MonthlyCartSeriesCollection.Clear();
                    LoadColumnSeries(MonthlyCartSeriesCollection);
                    ShowCartesianGraph(selectedTabItem, MonthlyCartSeriesCollection);
                }
                else if (selectedGraphType.Equals(GRAPH_TYPE_PIE))
                {
                    MonthlyPieSeriesCollection.Clear();
                    LoadPieSeries(MonthlyPieSeriesCollection);
                    ShowPieGraph(selectedTabItem, MonthlyPieSeriesCollection);
                }
                TotalMonthlyAmountLabel.Content = $"Celková částka za měsíc: {FinancesPerCategory.Sum(x => x.Total):N2} Kč";
            }
            else if (selectedTabItem.Equals(TAB_ITEM_QUART_OVERVIEW))
            {
                if (selectedGraphType.Equals(GRAPH_TYPE_CARTESIAN))
                {
                    QuartCartSeriesCollection.Clear();
                    LoadColumnSeries(QuartCartSeriesCollection);
                    ShowCartesianGraph(selectedTabItem, QuartCartSeriesCollection);
                }
                else if (selectedGraphType.Equals(GRAPH_TYPE_PIE))
                {
                    QuartPieSeriesCollection.Clear();
                    LoadPieSeries(QuartPieSeriesCollection);
                    ShowPieGraph(selectedTabItem, QuartPieSeriesCollection);
                }
                TotalQuartAmountLabel.Content = $"Celková částka za kvartál: {FinancesPerCategory.Sum(x => x.Total):N2} Kč";
            }
            else if (selectedTabItem.Equals(TAB_ITEM_YEARLY_OVERVIEW))
            {
                if (selectedGraphType.Equals(GRAPH_TYPE_CARTESIAN))
                {
                    YearlyCartSeriesCollection.Clear();
                    LoadColumnSeries(YearlyCartSeriesCollection);
                    ShowCartesianGraph(selectedTabItem, YearlyCartSeriesCollection);
                }
                else if (selectedGraphType.Equals(GRAPH_TYPE_PIE))
                {
                    YearlyPieSeriesCollection.Clear();
                    LoadPieSeries(YearlyPieSeriesCollection);
                    ShowPieGraph(selectedTabItem, YearlyPieSeriesCollection);
                }
                TotalYearlyAmountLabel.Content = $"Celková částka za rok: {FinancesPerCategory.Sum(x => x.Total):N2} Kč";
            }
        }

        // Načtení dat pro sloupcový graf
        private void LoadColumnSeries(SeriesCollection collection) 
        {
            foreach (var (Category, Total) in FinancesPerCategory)
            {
                ColumnSeries series = new ColumnSeries
                {
                    Title = Category,
                    Values = new ChartValues<double>()
                };
                series.Values.Add(Total);
                collection.Add(series);
            }
        }

        // Načtení dat pro koláčový graf
        private void LoadPieSeries(SeriesCollection collection)
        {
            foreach (var (Category, Total) in FinancesPerCategory)
            {
                PieSeries series = new PieSeries
                {
                    Title = Category,
                    Values = new ChartValues<double>()
                };
                series.Values.Add(Total);
                collection.Add(series);
            }
        }

        // Handlery pro zobrazení daného typu grafu grafu
        private void ShowCartesianGraph(string selectedTabItem, SeriesCollection collection)
        {
            if (selectedTabItem.Equals(TAB_ITEM_MONTHLY_OVERVIEW))
            {
                MonthlyCartChart.Visibility = Visibility.Visible;
                MonthlyPieChart.Visibility = Visibility.Hidden;
                MonthlyCartChart.Series = collection;
            }
            else if (selectedTabItem.Equals(TAB_ITEM_QUART_OVERVIEW))
            {
                QuartCartChart.Visibility = Visibility.Visible;
                QuartPieChart.Visibility = Visibility.Hidden;
                QuartCartChart.Series = collection;
            }
            else if (selectedTabItem.Equals(TAB_ITEM_YEARLY_OVERVIEW))
            {
                YearlyCartChart.Visibility = Visibility.Visible;
                YearlyPieChart.Visibility = Visibility.Hidden;
                YearlyCartChart.Series = collection;
            }
        }

        private void ShowPieGraph(string selectedTabItem, SeriesCollection collection)
        {
            if (selectedTabItem.Equals(TAB_ITEM_MONTHLY_OVERVIEW))
            {
                MonthlyPieChart.Visibility = Visibility.Visible;
                MonthlyCartChart.Visibility = Visibility.Hidden;
                MonthlyPieChart.Series = collection;
            }
            else if (selectedTabItem.Equals(TAB_ITEM_QUART_OVERVIEW))
            {
                QuartPieChart.Visibility = Visibility.Visible;
                QuartCartChart.Visibility = Visibility.Hidden;
                QuartPieChart.Series = collection;
            }
            else if (selectedTabItem.Equals(TAB_ITEM_YEARLY_OVERVIEW))
            {
                YearlyPieChart.Visibility = Visibility.Visible;
                YearlyCartChart.Visibility = Visibility.Hidden;
                YearlyPieChart.Series = collection;
            }
        }

        // Načtení hodnot do comboboxů
        private bool InitializeComboBoxes()
        {
            // Roky
            string username = LoggedUser.Username;
            List<int> years = userExpensesRepository.GetAvailableYearsForUser(username);
            if (years.Count == 0)
            { 
                Util.ShowInfoMessageBox("Zatím nemáte evidované žádné záznamy.\nPřejděte prosím na sekci 'Finance'");
                MonthlyPieChart.Visibility = Visibility.Hidden; 
                return false;
            }

            foreach (int year in years)
            {
                DashboardMonthlyYearComboBox.Items.Add(year.ToString());
                DashboardQuartYearComboBox.Items.Add(year.ToString());
                DashboardYearlyYearComboBox.Items.Add(year.ToString());
            }
            DashboardMonthlyYearComboBox.SelectedIndex = 0;
            DashboardQuartYearComboBox.SelectedIndex = 0;
            DashboardYearlyYearComboBox.SelectedIndex = 0;

            // Měsíce
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            int currentMonth = DateTime.Now.Month;
            for (int i = 0; i < 12; i++)
            {
                DashboardMonthlyMonthComboBox.Items.Add(monthNames[i]);
            }
            DashboardMonthlyYearComboBox.SelectedIndex = 0;
            DashboardMonthlyMonthComboBox.SelectedIndex = currentMonth - 1;

            // Typy grafů
            DashboardMonthlyGraphTypeComboBox.Items.Add(GRAPH_TYPE_CARTESIAN);
            DashboardMonthlyGraphTypeComboBox.Items.Add(GRAPH_TYPE_PIE);
            DashboardMonthlyGraphTypeComboBox.SelectedIndex = 1;            
            
            DashboardQuartGraphTypeComboBox.Items.Add(GRAPH_TYPE_CARTESIAN);
            DashboardQuartGraphTypeComboBox.Items.Add(GRAPH_TYPE_PIE);
            DashboardQuartGraphTypeComboBox.SelectedIndex = 1;            
            
            DashboardYearlyGraphTypeComboBox.Items.Add(GRAPH_TYPE_CARTESIAN);
            DashboardYearlyGraphTypeComboBox.Items.Add(GRAPH_TYPE_PIE);
            DashboardYearlyGraphTypeComboBox.SelectedIndex = 1;

            // Kvartály
            List<Quart> quarts = Util.GetQuarts();
            foreach (Quart quart in quarts)
            {
                DashboardQuartComboBox.Items.Add(quart);
            }
            DashboardQuartComboBox.SelectedIndex = 0;
            return true;
        }

        // Načtení uživatelských dat z databáze pro zobrazení grafu podle zvolené záložky
        public List<(string Category, double Total)> LoadFinanceDataByCategory(string username, int year, int month, string selectedTabItem)
        {
            string mode = selectedTabItem switch
            {
                var s when s == TAB_ITEM_MONTHLY_OVERVIEW => "monthly",
                var s when s == TAB_ITEM_QUART_OVERVIEW => "quarterly",
                var s when s == TAB_ITEM_YEARLY_OVERVIEW => "yearly",
                _ => "monthly"
            };

            try
            {
                return userExpensesRepository.GetUserExpensesByCategory(username, year, month, mode);
            }
            catch (Exception ex)
            {
                Logger.WriteErrorLog(this, $"Chyba při načítání financí: {ex.Message}");
                return [];
            }
        }

        // Zobrazení grafu po změně hodnot z comboboxů dle zvolené záložky
        private void DashboardShowGraphButton_Click(object sender, RoutedEventArgs e)
        {
            if (DashboardTabControl.SelectedItem is not TabItem selectedTab) return;
            string? tabHeader = selectedTab.Header.ToString();
            if (tabHeader == null) return;

            if (tabHeader.Equals(TAB_ITEM_MONTHLY_OVERVIEW))
            {
                int selectedMonth = DashboardMonthlyMonthComboBox.SelectedIndex + 1;
                int selectedYear = Convert.ToInt32(DashboardMonthlyYearComboBox.SelectedItem);
                string selectedGraphType = (string)DashboardMonthlyGraphTypeComboBox.SelectedItem;
                RefreshGraph(selectedYear, selectedMonth, selectedGraphType, tabHeader);
            }
            else if (tabHeader.Equals(TAB_ITEM_QUART_OVERVIEW))
            {
                Quart selectedQuart = (Quart)DashboardQuartComboBox.SelectedItem;
                int selectedMonth = selectedQuart.StartDate.Month;    
                int selectedYear = Convert.ToInt32(DashboardQuartYearComboBox.SelectedItem);
                string selectedGraphType = (string)DashboardQuartGraphTypeComboBox.SelectedItem;
                RefreshGraph(selectedYear, selectedMonth, selectedGraphType, tabHeader);
            }
            else if (tabHeader.Equals(TAB_ITEM_YEARLY_OVERVIEW))
            {
                int selectedYear = Convert.ToInt32(DashboardYearlyYearComboBox.SelectedItem);
                string selectedGraphType = (string)DashboardYearlyGraphTypeComboBox.SelectedItem;
                RefreshGraph(selectedYear, 0, selectedGraphType, tabHeader);
            }
        }
    }
}
