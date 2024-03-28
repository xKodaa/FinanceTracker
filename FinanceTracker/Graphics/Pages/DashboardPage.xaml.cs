using FinanceTracker.Model;
using FinanceTracker.Model.Config;
using FinanceTracker.Utility;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Mapping;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
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
    public partial class DashboardPage : Page
    {
        private MainWindow MainWindow { get; set; }
        public SeriesCollection PieSeriesCollection { get; set; }
        public SeriesCollection CartSeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private List<(string Category, double Total)> FinancesPerCategory;
        private DatabaseConnector Connector { get; set; }
        private User LoggedUser { get; set; }
        private readonly string GRAPH_TYPE_CARTESIAN = "Sloupcový";
        private readonly string GRAPH_TYPE_PIE = "Koláčový";

        public DashboardPage(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            InitializeComponent();
            Connector = DatabaseConnector.Instance;
            LoggedUser = Util.GetUser();
            PieSeriesCollection = [];
            CartSeriesCollection = [];
            Labels = [];
            FinancesPerCategory = [];
            Formatter = value => value.ToString("N");
            DateTime now = DateTime.Now;
            InitializeComboBoxes();
            RefreshGraph(now.Year, now.Month);  // Zobrazení grafu pro aktuální měsíc
        }

        private void RefreshGraph(int year, int month)
        {
            FinancesPerCategory = LoadFinanceDataByCategory(LoggedUser.Username, year, month);
            DisplayFinanceData();
            MainWindow.DataContext = this;
        }

        private void DisplayFinanceData()
        {
            string? selectedGraphType = DashboardGraphTypeComboBox.SelectedItem.ToString();
            if (selectedGraphType == null)
            {
                return;
            }


            if (selectedGraphType.Equals(GRAPH_TYPE_CARTESIAN))
            {
                CartSeriesCollection.Clear();
                ShowCartesianGraph();
           
                foreach (var (Category, Total) in FinancesPerCategory)
                {
                    ColumnSeries series = new ColumnSeries
                    {
                        Title = Category,
                        Values = new ChartValues<double>()
                    };
                    series.Values.Add(Total);
                    CartSeriesCollection.Add(series);
                }
            } 
            else if (selectedGraphType.Equals(GRAPH_TYPE_PIE))
            {
                PieSeriesCollection.Clear();
                ShowPieGraph();
                foreach (var (Category, Total) in FinancesPerCategory)
                {
                    PieSeries series = new PieSeries
                    {
                        Title = Category,
                        Values = new ChartValues<double>()
                    };
                    series.Values.Add(Total);
                    PieSeriesCollection.Add(series);
                }
            }
        }

        private void ShowCartesianGraph()
        {
            cartChart.Visibility = Visibility.Visible;
            pieChart.Visibility = Visibility.Hidden;
            cartChart.Series = CartSeriesCollection;
        }

        private void ShowPieGraph()
        {
            pieChart.Visibility = Visibility.Visible;
            cartChart.Visibility = Visibility.Hidden;
            pieChart.Series = PieSeriesCollection;
        }

        private void InitializeComboBoxes()
        {
            // Roky
            List<int> years = LoadFinanceYearsForUser();
            foreach (int year in years)
            {
                DashboardYearComboBox.Items.Add(year.ToString());
            }

            // Měsíce
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            int currentMonth = DateTime.Now.Month;
            for (int i = 0; i < 12; i++)
            {
                DashboardMonthComboBox.Items.Add(monthNames[i]);
            }
            DashboardYearComboBox.SelectedIndex = 0;
            DashboardMonthComboBox.SelectedIndex = currentMonth - 1;

            // Typ grafu
            DashboardGraphTypeComboBox.Items.Add(GRAPH_TYPE_CARTESIAN);
            DashboardGraphTypeComboBox.Items.Add(GRAPH_TYPE_PIE);
            DashboardGraphTypeComboBox.SelectedIndex = 1;
        }

        private List<int> LoadFinanceYearsForUser()
        {
            List<int> years = [];

            string sql = @"SELECT DISTINCT strftime('%Y', date) AS Year FROM UserFinances WHERE username = @username ORDER BY Year DESC";
            using (SQLiteCommand command = new SQLiteCommand(sql, Connector.Connection))
            {
                command.Parameters.AddWithValue("@username", LoggedUser.Username);

                using SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string? yearString = reader["Year"].ToString();
                    if (int.TryParse(yearString, out int year))
                    {
                        years.Add(year);
                    }
                }
            }
            return years;
        }

        public List<(string Category, double Total)> LoadFinanceDataByCategory(string username, int year, int month)
        {
            List<(string Category, double Total)> data = [];

            string sql = @"SELECT category, SUM(price) as TotalPrice 
                   FROM UserFinances 
                   WHERE username = @username AND strftime('%Y', date) = @year AND strftime('%m', date) = @month
                   GROUP BY category"; 
            using (SQLiteCommand command = new SQLiteCommand(sql, Connector.Connection))
            {
                command.Parameters.AddWithValue("@username", username); 
                command.Parameters.AddWithValue("@year", year.ToString());
                command.Parameters.AddWithValue("@month", month.ToString("D2"));
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string? category = reader["category"].ToString();
                    double? total = Convert.ToDouble(reader["TotalPrice"]);
                    if (category != null && total != null)
                        data.Add(((string Category, double Total))(Category: category, Total: total));
                }
            }
            return data;
        }

        private void DashboardShowGraphButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedMonth = DashboardMonthComboBox.SelectedIndex + 1;
            int selectedYear = Convert.ToInt32(DashboardYearComboBox.SelectedItem);
            RefreshGraph(selectedYear, selectedMonth);
        }
    }
}
