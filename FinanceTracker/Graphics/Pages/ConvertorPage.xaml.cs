using FinanceTracker.Model;
using FinanceTracker.Model.Services;
using FinanceTracker.Utility;
using System.Windows;
using System.Windows.Controls;


namespace FinanceTracker.Graphics.Pages
{
    public partial class ConvertorPage : Page
    {
        private CurrencyApiService CurrencyApiService { get; set; }

        public ConvertorPage(MainWindow mainWindow)
        {
            CurrencyApiService = new CurrencyApiService();
            InitializeComponent();
            LoadAvailableCurrencies();
            HideResultGrid();
        }

        // Načte asynchronně dostupné měny z API a vloží je do ComboBoxů
        private async void LoadAvailableCurrencies()
        {
            List<Currency> availableCurrencies = await CurrencyApiService.GetAvailableCurrenciesAsync();
            if (availableCurrencies.Count == 0) 
            { 
                Util.ShowErrorMessageBox("Měny se nenačetly z API, zkontrolujte připojení k internetu.");
                ConvertorBtnSubmit.IsEnabled = false;
                return;
            }
            ConvertorBtnSubmit.IsEnabled = true;

            Dispatcher.Invoke(() =>
            {
                SourceCurrencyComboBox.Items.Clear();
                TargetCurrencyComboBox.Items.Clear();
                foreach (var currency in availableCurrencies)
                {
                    SourceCurrencyComboBox.Items.Add(currency);
                    TargetCurrencyComboBox.Items.Add(currency);
                }
            });
        }

        // Kopírování výsledku do schránky
        private void ConvertorBtnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ConversionResultLabel.Content.ToString());
        }

        // Asynchronně provede konverzi měn a zobrazí výsledek
        private async void ConvertorBtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (SourceCurrencyComboBox.SelectedItem == null || TargetCurrencyComboBox.SelectedItem == null)
            {
                Util.ShowErrorMessageBox("Vyberte měnu");
                Logger.WriteErrorLog(this, "Uživatel nevybral měnu v převodníku");
                return;
            }
            string value = AmountTextBox.Text;
            if (decimal.TryParse(value.ToString(), out decimal amount))
            {
                Currency source = (Currency)SourceCurrencyComboBox.SelectedItem;
                Currency target = (Currency)TargetCurrencyComboBox.SelectedItem;
                string result = await CurrencyApiService.ConvertCurrencyAsync(source.Code, target.Code, amount);
                if (result == "") 
                {
                    Util.ShowErrorMessageBox("Nepovedlo se získat data, zkontrolujte připojení k internetu.");
                    Logger.WriteErrorLog(this, "Nepovedlo se získat data z API v převodníku");
                    return;
                }
                Dispatcher.Invoke(() =>
                {
                    ConversionResultLabel.Content = $"{amount} {source.Code} = {result}"; ;
                    ShowResultGrid();
                }); 
            }
            else
            {
                Util.ShowErrorMessageBox("Zadejte číslo");
                Logger.WriteErrorLog(this, "Uživatel nezadal číslo do boxu v převodníku");
            }
        }

        // Handlery pro zobrazení výsledku a kopírování výsledku
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
