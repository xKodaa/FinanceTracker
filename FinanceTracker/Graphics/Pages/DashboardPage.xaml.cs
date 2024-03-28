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
        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        private List<(string Category, double Total)> FinancesPerCategory;
        private DatabaseConnector Connector { get; set; }
        private User LoggedUser { get; set; }

        public DashboardPage(MainWindow mainWindow)
        {
            InitializeComponent();
            Connector = DatabaseConnector.Instance;
            LoggedUser = Util.GetUser();
            SeriesCollection = [];
            Labels = [];
            FinancesPerCategory = [];
            Formatter = value => value.ToString("N");
            DateTime now = DateTime.Now;
            InitializeComboBoxes();
            RefreshGraph(now.Year, now.Month);  // Zobrazení grafu pro aktuální měsíc
            mainWindow.DataContext = this;
        }

        private void RefreshGraph(int year, int month)
        {
            FinancesPerCategory = LoadFinanceDataByCategory(LoggedUser.Username, year, month);
            DisplayFinanceData();
        }   

        private void DisplayFinanceData()
        {
            //if (DashboardGraphTypeComboBox.SelectedItem)
            SeriesCollection.Clear();
            ColumnSeries series = new ColumnSeries
            {
                Title = "Kategorie",
                Values = new ChartValues<double>()
            };


            foreach (var (Category, Total) in FinancesPerCategory)
            {
                series.Values.Add(Total);
                Labels.Add(Category); 
            }
            SeriesCollection.Add(series);
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
            for (int i = 0; i < 12; i++)
            {
                DashboardMonthComboBox.Items.Add(monthNames[i]);
            }
            DashboardYearComboBox.SelectedIndex = 0;
            DashboardMonthComboBox.SelectedIndex = 0;

            // Typ grafu
            DashboardGraphTypeComboBox.Items.Add("Sloupcový");
            DashboardGraphTypeComboBox.Items.Add("Koláčový");
            DashboardGraphTypeComboBox.SelectedIndex = 0;
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
